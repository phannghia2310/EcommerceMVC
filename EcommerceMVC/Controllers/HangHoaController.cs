using AutoMapper;
using EcommerceMVC.Data;
using EcommerceMVC.Helpers;
using EcommerceMVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceMVC.Controllers
{
    public class HangHoaController : Controller
    {
        private readonly Ecommerce2024Context _context;
        private readonly IMapper _mapper;

        public HangHoaController(Ecommerce2024Context context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IActionResult Index(int? loai)
        {
            var hangHoas = _context.HangHoas.AsQueryable();

            if (loai.HasValue)
            {
                hangHoas = hangHoas.Where(p => p.MaLoai == loai.Value);
            }

            var result = hangHoas.Select(p => new HangHoaVM
            {
                MaHangHoa = p.MaHh,
                TenHangHoa = p.TenHh,
                Hinh = p.Hinh,
                DonGia = p.DonGia ?? 0,
                MoTaNgan = p.MoTaDonVi ?? "",
                TenLoai = p.MaLoaiNavigation.TenLoai
            });

            return View(result);
        }

        public IActionResult Search(string? query)
        {
            var hangHoas = _context.HangHoas.AsQueryable();

            if (query != null)
            {
                hangHoas = hangHoas.Where(p => p.TenHh.Contains(query));
            }

            var result = hangHoas.Select(p => new HangHoaVM
            {
                MaHangHoa = p.MaHh,
                TenHangHoa = p.TenHh,
                Hinh = p.Hinh,
                DonGia = p.DonGia ?? 0,
                MoTaNgan = p.MoTaDonVi ?? "",
                TenLoai = p.MaLoaiNavigation.TenLoai
            });

            return View(result);
        }

        public IActionResult Detail(int id)
        {
            var customerId = HttpContext.User.Claims.FirstOrDefault(kh => kh.Type == MySetting.CLAIM_CUSTOMERID);
            if (customerId != null)
            {
                ViewBag.CustomerId = customerId.ToString();
            }

            var data = _context.HangHoas
                .Include(p => p.MaLoaiNavigation)
                .SingleOrDefault(p => p.MaHh == id);
            if (data == null)
            {
                TempData["Message"] = $"Không tìm thấy sản phẩm có mã {id}";
                return Redirect("/404");
            }        

            double? diemTB = _context.YeuThiches.Where(hh => hh.MaHh == id).Select(hh => (double?)hh.DiemDanhGia).Average();

            var result = new ChiTietHangHoaVM
            {
                MaHangHoa = data.MaHh,
                TenHangHoa = data.TenHh,
                Hinh = data.Hinh ?? string.Empty,
                MoTaNgan = data.MoTaDonVi ?? string.Empty,
                TenLoai = data.MaLoaiNavigation.TenLoai,
                ChiTiet = data.MoTa ?? string.Empty,
                DonGia = data.DonGia ?? 0,
                DiemDanhGia = Convert.ToInt32(diemTB),
                SoLuongTon = data.SoLuongTon
            };

            return View(result);
        }

        [Authorize(AuthenticationSchemes = "CustomerAuth")]
        [HttpPost]
        public IActionResult PostComment(DanhGiaVM model, int id)
        {
            if(ModelState.IsValid)
            {
                var customerId = HttpContext.User.Claims.FirstOrDefault(kh => kh.Type == MySetting.CLAIM_CUSTOMERID).Value;
                var donHang = _context.HoaDons.Where(hd => hd.MaKh == customerId && hd.MaTrangThai == 3).ToList();

                foreach(var item in donHang)
                {
                    var ctdh = _context.ChiTietHds.Where(ct => ct.MaHd == item.MaHd).ToList();
                    foreach(var ct in ctdh)
                    {
                        if(ct.MaHh == id)
                        {
                            var danhGia = _mapper.Map<YeuThich>(model);
                            danhGia.MaKh = customerId;
                            danhGia.MaHh = id;
                            danhGia.NgayChon = DateTime.Now;
                            danhGia.DiemDanhGia = model.DiemDanhGia;
                            danhGia.MoTa = model.MoTa;
                            danhGia.MaKhNavigation = _context.KhachHangs.FirstOrDefault(kh => kh.MaKh == customerId);
                            danhGia.MaHhNavigation = _context.HangHoas.FirstOrDefault(hh => hh.MaHh == id);

                            _context.YeuThiches.Add(danhGia);
                            _context.SaveChanges();
                        }
                        else
                        {
                            TempData["ErrorComment"] = "Bạn chưa bao giờ đặt sản phẩm này nên không thể viết đánh giá.";
                            TempData["ProductId"] = id;
                            return RedirectToAction("Detail", "HangHoa", new { id = id });
                        }
                    }
                }
                
                
            }

            return RedirectToAction("Detail", "HangHoa", new {id = id});
        }
    }
}
