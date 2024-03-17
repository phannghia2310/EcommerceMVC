using EcommerceMVC.Data;
using EcommerceMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using EcommerceMVC.Helpers;
using Microsoft.AspNetCore.Authorization;
using EcommerceMVC.Services;
using System.Text;
using Org.BouncyCastle.Crypto.Engines;
using EcommerceMVC.Areas.Admin.Models;
using Microsoft.EntityFrameworkCore;

namespace EcommerceMVC.Controllers
{
    public class CartController : Controller
    {
        private readonly PaypalClient _paypalClient;
        public readonly Ecommerce2024Context _context;
        private readonly IVnPayService _vnPayService;
        private readonly IEmailSender _emailSender;

        public CartController(Ecommerce2024Context context, PaypalClient paypalClient, IVnPayService vnPayService, IEmailSender emailSender)
        {
            _paypalClient = paypalClient;
            _context = context;
            _vnPayService = vnPayService;
            _emailSender = emailSender;
        }

        public List<CartItem> Cart => HttpContext.Session.Get<List<CartItem>>(MySetting.CART_KEY) ?? new List<CartItem>();
        public ThanhToanVM paymentModel => HttpContext.Session.Get<ThanhToanVM>(MySetting.PAYMENT_KEY) ?? new ThanhToanVM();

        public IActionResult Index()
        {
            return View(Cart);
        }

        public IActionResult AddToCart(int id, int quantity = 1)
        {
            var gioHang = Cart;
            var item = gioHang.SingleOrDefault(p => p.MaHh == id);
            if (item == null)
            {
                var hangHoa = _context.HangHoas.SingleOrDefault(p => p.MaHh == id);
                if (hangHoa == null)
                {
                    TempData["Message"] = $"Không tìm thấy hàng hóa có mã {id}";
                    return Redirect("/404");
                }

                item = new CartItem
                {
                    MaHh = hangHoa.MaHh,
                    TenHh = hangHoa.TenHh,
                    DonGia = hangHoa.DonGia ?? 0,
                    Hinh = hangHoa.Hinh ?? string.Empty,
                    SoLuong = quantity
                };

                gioHang.Add(item);
            }
            else
            {
                item.SoLuong += quantity;
            }

            HttpContext.Session.Set(MySetting.CART_KEY, gioHang);

            return RedirectToAction("Index");
        }

        public IActionResult MinusProduct(int id, int quantity = 1)
        {
            var gioHang = Cart;
            var item = gioHang.SingleOrDefault(p => p.MaHh == id);
            if(item != null)
            {
                if(item.SoLuong > quantity)
                {
                    item.SoLuong -= quantity;
                }
                else
                {
                    gioHang.Remove(item);
                }

                HttpContext.Session.Set(MySetting.CART_KEY, gioHang);
            }

            return RedirectToAction("Index");
        }

        public IActionResult RemoveCart(int id)
        {
            var gioHang = Cart;
            var item = gioHang.SingleOrDefault(p => p.MaHh == id);
            if(item != null)
            {
                gioHang.Remove(item);
                HttpContext.Session.Set(MySetting.CART_KEY, gioHang);
            }

            return RedirectToAction("Index");
        }

        [Authorize(AuthenticationSchemes = "CustomerAuth")]
        [HttpGet]
        public IActionResult ThanhToan()
        {
            var gioHang = Cart;
            if (gioHang.Count == 0)
            {
                return Redirect("/");
            }

            ViewBag.PaypalClientId = _paypalClient.ClientId;
            return  View(Cart);
        }

        [Authorize(AuthenticationSchemes = "CustomerAuth")]
        [HttpPost]
        public IActionResult ThanhToan(ThanhToanVM model, string payment = "COD")
        {
            if(ModelState.IsValid)
            {
                HttpContext.Session.Set(MySetting.PAYMENT_KEY, model);

                if (payment == "Thanh toán bằng VNPAY")
                {
                    var vnpayModel = new VnPaymentRequestModel
                    {
                        Amount = Cart.Sum(p => p.ThanhTien) * 20000,
                        CreateDate = DateTime.Now,
                        Descripstion = $"{model.HoTen} - {model.DienThoai}",
                        FullName = model.HoTen,
                        OrderId = new Random().Next(1000, 100000)
                    };

                    return Redirect(_vnPayService.CreatePaymentUrl(HttpContext, vnpayModel));
                }

                var customerIdClaim = HttpContext.User.Claims.SingleOrDefault(p => p.Type == MySetting.CLAIM_CUSTOMERID);
                if (customerIdClaim == null)
                {
                    return RedirectToAction("DangNhap", "KhachHang");
                }

                var customerId = customerIdClaim.Value;
                var khachHang = new KhachHang();
                if(model.GiongKhachHang)
                {
                    khachHang = _context.KhachHangs.SingleOrDefault(kh => kh.MaKh == customerId);
                }
                var hoaDon = new HoaDon
                {
                    MaKh = customerId,
                    HoTen = model.HoTen ?? khachHang?.HoTen,
                    DiaChi = model.DiaChi ?? khachHang.DiaChi,
                    SoDienThoai = model.DienThoai ?? khachHang.DienThoai,
                    NgayDat = DateTime.Now,
                    NgayGiao = DateTime.Now.AddDays(6),
                    CachThanhToan = "COD",
                    CachVanChuyen = "GRAB",
                    MaTrangThai = 0,
                    GhiChu = model.GhiChu
                };

                _context.Database.BeginTransaction();
                try
                {
                    _context.Database.CommitTransaction();
                    _context.Add(hoaDon);
                    _context.SaveChanges();

                    var cthds = new List<ChiTietHd>();
                    foreach(var item in Cart)
                    {
                        cthds.Add(new ChiTietHd
                        {
                            MaHd = hoaDon.MaHd,
                            SoLuong = item.SoLuong,
                            DonGia = item.DonGia,
                            MaHh = item.MaHh,
                            MaHhNavigation = _context.HangHoas.FirstOrDefault(hh => hh.MaHh == item.MaHh),
                            GiamGia = 0
                        }); ;
                    }

                    _context.AddRange(cthds);
                    _context.SaveChanges();

                    HttpContext.Session.Set<List<CartItem>>(MySetting.CART_KEY, new List<CartItem>());

                    var tongTien = _context.ChiTietHds.Where(ct => ct.MaHd == hoaDon.MaHd).Sum(ct => (ct.DonGia * ct.SoLuong) - (ct.DonGia * ct.GiamGia));
                    var htmlTable = new StringBuilder();
                    htmlTable.Append("<style>table {border-collapse: separate; border-spacing: 10px;} table td, table th {padding: 10px;}</style>");
                    htmlTable.Append("<table>");
                    htmlTable.Append("<tr><th>Mã chi tiết</th><th>Tên sản phẩm</th><th>Hình</th><th>Giá</th><th>Số lượng</th><th>Giảm giá</th><th>Thành tiền</th></tr>");
                    foreach(var ct in cthds)
                    {
                        htmlTable.Append($"<tr><td>{ct.MaCt}</td><td>{ct.MaHhNavigation.TenHh}</td><td><img src='https://localhost:7006/Hinh/HangHoa/{ct.MaHhNavigation.Hinh}' /></td><td>{ct.DonGia}</td><td>{ct.SoLuong}</td><td>{ct.GiamGia}</td><td>{(ct.DonGia * ct.SoLuong) - (ct.DonGia * ct.GiamGia)}</td></tr>");
                    }
                    htmlTable.Append("</table>");

                    var receiver = _context.KhachHangs.SingleOrDefault(kh => kh.MaKh == customerId).Email;
                    var subject = $"Thông tin đơn hàng: {hoaDon.MaHd}";
                    var message = $"Xin chào {_context.KhachHangs.SingleOrDefault(kh => kh.MaKh == customerId).HoTen},\nDưới đây là thông tin chi tiết đơn hàng của bạn.\n\nMã đơn hàng: {hoaDon.MaHd}\nNgày đặt: {hoaDon.NgayDat}\nNgày giao (dự kiến): {hoaDon.NgayGiao}\n\n{htmlTable}\n\nTổng tiền: {tongTien}\n\nPhương thức thanh toán: {hoaDon.CachThanhToan}";

                    _emailSender.SendEmailAsync(receiver, subject, message);

                    return View("Success");
                }
                catch
                {
                    _context.Database.RollbackTransaction();
                }
            }

            return View(Cart);
        }

        [Authorize(AuthenticationSchemes = "CustomerAuth")]
        public IActionResult PaymentSuccess()
        {
            return View("Success");
        }

        #region Paypal payment
        [Authorize(AuthenticationSchemes = "CustomerAuth")]
        [HttpPost("/Cart/SaveForm")]
        public IActionResult SaveForm([FromForm] ThanhToanVM model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Lưu model vào session
            HttpContext.Session.Set(MySetting.PAYMENT_KEY, model);

            return Ok();
        }

        [Authorize(AuthenticationSchemes = "CustomerAuth")]
        [HttpPost("/Cart/CreatePaypalOrder")]
        public async Task<IActionResult> CreatePaypalOrder(CancellationToken cancellationToken)
        {
            var tongTien = Cart.Sum(p => p.ThanhTien).ToString();
            var donViTienTe = "USD";
            var maDonHangThanChieu = "DH" + DateTime.Now.Ticks.ToString();

            try
            {
                var response = await _paypalClient.CreateOrder(tongTien, donViTienTe, maDonHangThanChieu);
                return Ok(response);
            } 
            catch (Exception ex)
            {
                var error = new { ex.GetBaseException().Message };
                return BadRequest(error);
            }
        }

        [Authorize(AuthenticationSchemes = "CustomerAuth")]
        [HttpPost("/Cart/CapturePaypalOrder")]
        public async Task<IActionResult> CapturePaypalOrder(string orderID, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _paypalClient.CaptureOrder(orderID);

                //Save to database
                ThanhToanVM model = paymentModel;
                var customerIdClaim = HttpContext.User.Claims.SingleOrDefault(p => p.Type == MySetting.CLAIM_CUSTOMERID);
                if(customerIdClaim == null)
                {
                    return RedirectToAction("DangNhap", "KhachHang");
                }

                var customerId = customerIdClaim.Value;
                var khachHang = new KhachHang();
                if (model.GiongKhachHang)
                {
                    khachHang = _context.KhachHangs.SingleOrDefault(kh => kh.MaKh == customerId);
                }

                var hoaDon = new HoaDon
                {
                    MaKh = customerId,
                    HoTen = model.HoTen ?? khachHang?.HoTen,
                    DiaChi = model.DiaChi ?? khachHang.DiaChi,
                    SoDienThoai = model.DienThoai ?? khachHang.DienThoai,
                    NgayDat = DateTime.Now,
                    NgayGiao = DateTime.Now.AddDays(6),
                    CachThanhToan = "PAYPAL",
                    CachVanChuyen = "GRAB",
                    MaTrangThai = 1,
                    GhiChu = model.GhiChu
                };

                _context.Database.BeginTransaction();
                try
                {
                    _context.Database.CommitTransaction();
                    _context.Add(hoaDon);
                    _context.SaveChanges();

                    var cthds = new List<ChiTietHd>();
                    foreach (var item in Cart)
                    {
                        cthds.Add(new ChiTietHd
                        {
                            MaHd = hoaDon.MaHd,
                            SoLuong = item.SoLuong,
                            DonGia = item.DonGia,
                            MaHh = item.MaHh,
                            GiamGia = 0
                        });
                    }

                    _context.AddRange(cthds);
                    _context.SaveChanges();

                    HttpContext.Session.Set<List<CartItem>>(MySetting.CART_KEY, new List<CartItem>());
                    HttpContext.Session.Set<ThanhToanVM>(MySetting.PAYMENT_KEY, new ThanhToanVM());

                    var tongTien = _context.ChiTietHds.Where(ct => ct.MaHd == hoaDon.MaHd).Sum(ct => (ct.DonGia * ct.SoLuong) - (ct.DonGia * ct.GiamGia));
                    var htmlTable = new StringBuilder();
                    htmlTable.Append("<style>table {border-collapse: separate; border-spacing: 10px;} table td, table th {padding: 10px;}</style>");
                    htmlTable.Append("<table>");
                    htmlTable.Append("<tr><th>Mã chi tiết</th><th>Tên sản phẩm</th><th>Hình</th><th>Giá</th><th>Số lượng</th><th>Giảm giá</th><th>Thành tiền</th></tr>");
                    foreach (var ct in cthds)
                    {
                        htmlTable.Append($"<tr><td>{ct.MaCt}</td><td>{ct.MaHhNavigation.TenHh}</td><td><img src='https://localhost:7006/Hinh/HangHoa/{ct.MaHhNavigation.Hinh}' /></td><td>{ct.DonGia}</td><td>{ct.SoLuong}</td><td>{ct.GiamGia}</td><td>{(ct.DonGia * ct.SoLuong) - (ct.DonGia * ct.GiamGia)}</td></tr>");
                    }
                    htmlTable.Append("</table>");

                    var receiver = _context.KhachHangs.SingleOrDefault(kh => kh.MaKh == customerId).Email;
                    var subject = $"Thông tin đơn hàng: {hoaDon.MaHd}";
                    var message = $"Xin chào {_context.KhachHangs.SingleOrDefault(kh => kh.MaKh == customerId).HoTen},\nDưới đây là thông tin chi tiết đơn hàng của bạn.\n\nMã đơn hàng: {hoaDon.MaHd}\nNgày đặt: {hoaDon.NgayDat}\nNgày giao (dự kiến): {hoaDon.NgayGiao}\n\n{htmlTable}\n\nTổng tiền: {tongTien}\n\nPhương thức thanh toán: {hoaDon.CachThanhToan}";

                    _emailSender.SendEmailAsync(receiver, subject, message);

                    TempData["Message"] = "Thanh toán PayPal thành công";
                    return Ok(response);
                }
                catch
                {
                    _context.Database.RollbackTransaction();
                }
            }
            catch (Exception ex)
            {
                var error = new { ex.GetBaseException().Message };
                return BadRequest(error);
            }

            return View(Cart);
        }
        #endregion

        [Authorize(AuthenticationSchemes = "CustomerAuth")]
        public IActionResult PaymentFail()
        {
            return View();
        }

        [Authorize(AuthenticationSchemes = "CustomerAuth")]
        public IActionResult PaymentCallBack()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);

            if(response == null || response.VnPayResponseCode != "00")
            {
                TempData["Message"] = $"Lỗi thanh toán VNPAY: {response.VnPayResponseCode}";
                return RedirectToAction("PaymentFail");
            }

            //Save to database 
            ThanhToanVM model = paymentModel;
            var customerIdClaim = HttpContext.User.Claims.SingleOrDefault(p => p.Type == MySetting.CLAIM_CUSTOMERID);
            if(customerIdClaim == null)
            {
                return RedirectToAction("DangNhap", "KhachHang");
            }

            var customerId = customerIdClaim.Value;
            var khachHang = new KhachHang();
            if (model.GiongKhachHang)
            {
                khachHang = _context.KhachHangs.SingleOrDefault(kh => kh.MaKh == customerId);
            }

            var hoaDon = new HoaDon
            {
                MaKh = customerId,
                HoTen = model.HoTen ?? khachHang?.HoTen,
                DiaChi = model.DiaChi ?? khachHang.DiaChi,
                SoDienThoai = model.DienThoai ?? khachHang.DienThoai,
                NgayDat = DateTime.Now,
                NgayGiao = DateTime.Now.AddDays(6),
                CachThanhToan = "VNPAY",
                CachVanChuyen = "GRAB",
                MaTrangThai = 1,
                GhiChu = model.GhiChu
            };

            _context.Database.BeginTransaction();
            try
            {
                _context.Database.CommitTransaction();
                _context.Add(hoaDon);
                _context.SaveChanges();

                var cthds = new List<ChiTietHd>();
                foreach (var item in Cart)
                {
                    cthds.Add(new ChiTietHd
                    {
                        MaHd = hoaDon.MaHd,
                        SoLuong = item.SoLuong,
                        DonGia = item.DonGia,
                        MaHh = item.MaHh,
                        GiamGia = 0
                    });
                }

                _context.AddRange(cthds);
                _context.SaveChanges();

                HttpContext.Session.Set<List<CartItem>>(MySetting.CART_KEY, new List<CartItem>());
                HttpContext.Session.Set<ThanhToanVM>(MySetting.PAYMENT_KEY, new ThanhToanVM());

                var tongTien = _context.ChiTietHds.Where(ct => ct.MaHd == hoaDon.MaHd).Sum(ct => (ct.DonGia * ct.SoLuong) - (ct.DonGia * ct.GiamGia));
                var htmlTable = new StringBuilder();
                htmlTable.Append("<style>table {border-collapse: separate; border-spacing: 10px;} table td, table th {padding: 10px;}</style>");
                htmlTable.Append("<table>");
                htmlTable.Append("<tr><th>Mã chi tiết</th><th>Tên sản phẩm</th><th>Hình</th><th>Giá</th><th>Số lượng</th><th>Giảm giá</th><th>Thành tiền</th></tr>");
                foreach (var ct in cthds)
                {
                    htmlTable.Append($"<tr><td>{ct.MaCt}</td><td>{ct.MaHhNavigation.TenHh}</td><td><img src='https://localhost:7006/Hinh/HangHoa/{ct.MaHhNavigation.Hinh}' /></td><td>{ct.DonGia}</td><td>{ct.SoLuong}</td><td>{ct.GiamGia}</td><td>{(ct.DonGia * ct.SoLuong) - (ct.DonGia * ct.GiamGia)}</td></tr>");
                }
                htmlTable.Append("</table>");

                var receiver = _context.KhachHangs.SingleOrDefault(kh => kh.MaKh == customerId).Email;
                var subject = $"Thông tin đơn hàng: {hoaDon.MaHd}";
                var message = $"Xin chào {_context.KhachHangs.SingleOrDefault(kh => kh.MaKh == customerId).HoTen},\nDưới đây là thông tin chi tiết đơn hàng của bạn.\n\nMã đơn hàng: {hoaDon.MaHd}\nNgày đặt: {hoaDon.NgayDat}\nNgày giao (dự kiến): {hoaDon.NgayGiao}\n\n{htmlTable}\n\nTổng tiền: {tongTien}\n\nPhương thức thanh toán: {hoaDon.CachThanhToan}";

                _emailSender.SendEmailAsync(receiver, subject, message);

                TempData["Message"] = "Thanh toán VNPAY thành công";
                return RedirectToAction("PaymentSuccess");
            }
            catch
            {
                _context.Database.RollbackTransaction();
            }

            return View(Cart);
        }

        [Authorize(AuthenticationSchemes = "CustomerAuth")]
        [HttpGet]
        public IActionResult History()
        {
            var trangThais =  _context.TrangThais.ToList();

            var result = trangThais.Select(tt => new TrangThaiVM
            {
                MaTrangThai = tt.MaTrangThai,
                TenTrangThai = tt.TenTrangThai,
                MoTa = tt.MoTa
            });
            return View(result);
        }

        [Authorize(AuthenticationSchemes = "CustomerAuth")]
        public IActionResult GetOrderByStatus(int id)
        {
            var claim = HttpContext.User.Claims.FirstOrDefault(p => p.Type == MySetting.CLAIM_CUSTOMERID);
            var hoaDons = new List<HoaDonVM>();

            if (claim != null)
            {
                var customerId = claim.Value;

                hoaDons = _context.HoaDons.Where(hd => hd.MaTrangThai == id && hd.MaKh == customerId).Select(hd => new HoaDonVM
                {
                    MaHd = hd.MaHd,
                    NgayDat = hd.NgayDat,
                    NgayGiao = hd.NgayGiao,
                    CachThanhToan = hd.CachThanhToan,
                    MaTrangThai = hd.MaTrangThai,
                }).ToList();
            }

            return PartialView("OrderByStatus", hoaDons);
        }

        public IActionResult ChangeStatus(int? id)
        {
            var hoaDon = _context.HoaDons.Find(id);
            if(hoaDon.MaTrangThai == 0)
            {
                hoaDon.MaTrangThai = -1;
            }
            else if(hoaDon.MaTrangThai == 2)
            {
                hoaDon.MaTrangThai = 3;
                hoaDon.NgayGiao = DateTime.Now;
            }

            _context.Update(hoaDon);
            _context.SaveChanges();

            return RedirectToAction("History", "Cart");
        }

        public IActionResult OrderDetails(int? id)
        {
            var listCthd = _context.ChiTietHds.AsQueryable();
            
            if(id.HasValue)
            {
                ViewBag.MaHd = id;
                listCthd = listCthd.Where(ct => ct.MaHdNavigation.MaHd == id);
            }

            var result = listCthd.Select(ct => new ChiTietHdVM
            {
                MaCT = ct.MaCt,
                TenHh = ct.MaHhNavigation.TenHh,
                Hinh = ct.MaHhNavigation.Hinh,
                DonGia = ct.DonGia,
                SoLuong = ct.SoLuong,
                GiamGia = ct.GiamGia
            });

            return View(result);
        }
    }
}
