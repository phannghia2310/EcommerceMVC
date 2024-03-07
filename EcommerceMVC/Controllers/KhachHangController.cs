using AutoMapper;
using EcommerceMVC.Data;
using EcommerceMVC.Helpers;
using EcommerceMVC.Services;
using EcommerceMVC.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Policy;

namespace EcommerceMVC.Controllers
{
    public class KhachHangController : Controller
    {
        private readonly Ecommerce2024Context _context;
        private readonly IMapper _mapper;
        private readonly Services.IEmailSender _emailSender;

        public KhachHangController(Ecommerce2024Context context, IMapper mapper, Services.IEmailSender emailSender)
        {
            _context = context;
            _mapper = mapper;
            _emailSender = emailSender;
        }

        #region DangKy
        [HttpGet]
        public IActionResult DangKy()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DangKy(DangKyVM model, IFormFile Hinh)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var check = _context.KhachHangs.FirstOrDefault(kh => kh.MaKh == model.MaKh);
                    if (check == null)
                    {
                        var khachHang = _mapper.Map<KhachHang>(model);
                        khachHang.RandomKey = MyUtil.GenerateRandomKey();
                        khachHang.MatKhau = model.MatKhau.ToMd5Hash(khachHang.RandomKey);
                        khachHang.HieuLuc = false;
                        khachHang.VaiTro = 0;

                        if (Hinh != null)
                        {
                            khachHang.Hinh = MyUtil.UploadHinh(Hinh, "KhachHang");
                        }

                        _context.KhachHangs.Add(khachHang);
                        await _context.SaveChangesAsync();

                        var activationLink = Url.Action("KichHoatTaiKhoan", "KhachHang", new { token = khachHang.RandomKey }, Request.Scheme);
                        var receiver = model.Email;
                        var subject = "Kích hoạt tài khoản";
                        var message = $"Xin chào {model.HoTen},\n\nVui lòng nhấn vào liên kết sau để kích hoạt tài khoản: {activationLink}";


                        _emailSender?.SendEmailAsync(receiver, subject, message);

                        return RedirectToAction("DangNhap", "KhachHang");
                    }
                    else
                    {
                        ViewBag.ErrorRegister = "Tên đăng nhập đã tồn tại";
                        return View();
                    }
                } 
                catch (Exception ex)
                {
                    var mes = $"{ex.Message}";
                }
            }
            return View();
        }

        [HttpGet]
        public IActionResult KichHoatTaiKhoan(string token) 
        {
            var khachHang = _context.KhachHangs.FirstOrDefault(kh => kh.RandomKey == token);
            if(khachHang != null)
            {
                khachHang.HieuLuc = true;
                _context.Update(khachHang);
                _context.SaveChanges();
                ViewBag.SuccessActivation = "Tài khoản của bạn đã được kích hoạt thành công!";
            }
            else
            {
                ViewBag.ErrorActivation = "Không thể kích hoạt tài khoản. Liên kết không hợp lệ.";
            }
            return RedirectToAction("DangNhap", "KhachHang");
        }
        #endregion

        #region DangNhap
        [HttpGet]
        public IActionResult DangNhap()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DangNhap(DangNhapVM model)
        {
            if (ModelState.IsValid)
            {
                var khachHang = _context.KhachHangs.SingleOrDefault(kh => kh.MaKh == model.UserName);
                if(khachHang == null)
                {
                    ModelState.AddModelError("loi", "Không tồn tại khách hàng này.");
                }
                else
                {
                    if(!khachHang.HieuLuc)
                    {
                        ModelState.AddModelError("loi", "Tài khoản đã bị khóa. Vui lòng liên hệ nhân viên CSKH.");
                    }
                    else
                    {
                        if(khachHang.MatKhau != model.Password.ToMd5Hash(khachHang.RandomKey))
                        {
                            ModelState.AddModelError("loi", "Sai mật khẩu.");
                        }
                        else
                        {
                            var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Email, khachHang.Email),
                                new Claim(ClaimTypes.Name, khachHang.HoTen),
                                new Claim(MySetting.CLAIM_CUSTOMERID, khachHang.MaKh),

                                //claim - role động
                                new Claim(ClaimTypes.Role, "Customer"),
                            }; 

                            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                            await HttpContext.SignInAsync(claimsPrincipal);

                            return Redirect("/");
                        }
                    }
                }
            }
            return View();
        }
        #endregion

        #region Google
        public IActionResult GoogleLogin(string returnUrl = "/")
        {
            var properties = new AuthenticationProperties { RedirectUri = Url.Action("GoogleResponse", "KhachHang", new {ReturnUrl = returnUrl}) };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        public async Task<IActionResult> GoogleResponse(string returnUrl = "/")
        {

            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (result.Succeeded)
            {
                var customerId = result.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value.ToString();

                var check = _context.KhachHangs.SingleOrDefault(kh => kh.MaKh == customerId);
                if (check == null)
                {
                    var khachHang = new KhachHang();
                    khachHang.MaKh = customerId;
                    khachHang.HoTen = result.Principal.FindFirst(ClaimTypes.Name)?.Value;
                    khachHang.Email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;
                    khachHang.HieuLuc = true;
                    khachHang.VaiTro = 0;

                    _context.KhachHangs.Add(khachHang);
                    await _context.SaveChangesAsync();

                    return Redirect("/");
                }
                else
                {
                    var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Email, check.Email),
                            new Claim(ClaimTypes.Name, check.HoTen),
                            new Claim(MySetting.CLAIM_CUSTOMERID, check.MaKh),

                            //claim - role động
                            new Claim(ClaimTypes.Role, "Customer"),
                        };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                    await HttpContext.SignInAsync(claimsPrincipal);

                    return Redirect("/");
                }
            }

            return LocalRedirect(returnUrl);
        }
        #endregion

        [Authorize]
        [HttpGet]
        public IActionResult Profile()
        {
            var customerId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == MySetting.CLAIM_CUSTOMERID).Value;
            if (customerId == null)
            {
                TempData["Message"] = "Vui lòng đăng nhập để xem thông tin cá nhân";
                return Redirect("/DangNhap");
            }

            var data = _context.KhachHangs.SingleOrDefault(kh => kh.MaKh == customerId);
            if(data == null)
            {
                TempData["Message"] = $"Không tìm thấy khách hàng";
                return Redirect("/404");
            }

            var result = new KhachHangVM
            {
                MaKh = data.MaKh,
                HoTen = data.HoTen,
                GioiTinh = data.GioiTinh,
                NgaySinh = data.NgaySinh,
                DiaChi = data.DiaChi,
                DienThoai = data.DienThoai,
                Email = data.Email,
                Hinh = data.Hinh
            };

            return View(result);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Profile(KhachHangVM model, IFormFile Hinh)
        {
            var customerId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == MySetting.CLAIM_CUSTOMERID).Value;
            var khachHang = _context.KhachHangs.SingleOrDefault(kh => kh.MaKh == customerId);
            var mapModel = _mapper.Map<KhachHang>(model);

            if (khachHang != null)
            {
                khachHang.HoTen = mapModel.HoTen;
                khachHang.Email = mapModel.Email;
                khachHang.GioiTinh = mapModel.GioiTinh;
                khachHang.NgaySinh = mapModel.NgaySinh;
                khachHang.DiaChi = mapModel.DiaChi;
                khachHang.DienThoai = mapModel.DienThoai;

                if(Hinh != null)
                {
                    khachHang.Hinh = MyUtil.UploadHinh(Hinh, "KhachHang");
                }

                _context.KhachHangs.Update(khachHang);
                _context.SaveChanges();

                ViewBag.SuccessUpdate = "Cập nhật thông tin thành công!";
            }
            else
            {
                ViewBag.ErrorUpdate = "Không tìm thấy thông tin khách hàng!";
            }

            return View(_mapper.Map<KhachHangVM>(khachHang));
        }

        #region DoiMatKhau
        [Authorize]
        public IActionResult NhapMailXacThuc()
        {
            return View();
        }

        [Authorize]
        public IActionResult GuiMailXacThuc(KhachHangVM model)
        {
            var customerId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == MySetting.CLAIM_CUSTOMERID).Value;
            var khachHang = _context.KhachHangs.SingleOrDefault(kh => kh.MaKh == customerId);

            if (model.Email != khachHang.Email)
            {
                ViewBag.EmailErorr = "Email xác thực không trùng khớp";
                return View("NhapMailXacThuc");
            }
            else
            {
                var activationLink = Url.Action("XacThucEmail", "KhachHang", new { token = khachHang.RandomKey }, Request.Scheme);
                var receiver = model.Email;
                var subject = "Xác thực tài khoản";
                var message = $"Xin chào {model.HoTen},\n\nVui lòng nhấn vào liên kết sau để xác thực tài khoản: {activationLink}";

                _emailSender?.SendEmailAsync(receiver, subject, message);
            }

            return View("Loading");
        }

        [Authorize]
        [HttpGet]
        public IActionResult XacThucEmail(string token)
        {
            var khachHang = _context.KhachHangs.FirstOrDefault(kh => kh.RandomKey == token);
            if(khachHang == null)
            {
                ViewBag.ErrorActivation = "Không thể xác thực tài khoản. Liên kết không hợp lệ.";
                return RedirectToAction("NhapEmailXacThuc", "KhachHang");
            }

            return RedirectToAction("DoiMatKhau", "KhachHang");
        }

        [HttpGet]
        public IActionResult DoiMatKhau()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DoiMatKhau(KhachHangVM model, string newPassword, string rePassword)
        {
            var customerId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == MySetting.CLAIM_CUSTOMERID).Value;
            var khachHang = _context.KhachHangs.SingleOrDefault(kh => kh.MaKh == customerId);

            if(khachHang.MatKhau != model.MatKhau.ToMd5Hash(khachHang.RandomKey))
            {
                ViewBag.ErrorOldPass = "Mật khẩu cũ không đúng";
            }
            else
            {
                if(newPassword == null || newPassword == "")
                {
                    ViewBag.ErrorNewPass = "Mật khẩu mới trống";
                }
                else
                {
                    if (newPassword != rePassword)
                    {
                        ViewBag.ErrorRePass = "Mật khẩu nhập lại không khớp";
                    }
                    else
                    {
                        khachHang.MatKhau = newPassword.ToMd5Hash(khachHang.RandomKey);
                        _context.Update(khachHang);
                        _context.SaveChanges();

                        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                        return RedirectToAction("DangNhap", "KhachHang");
                    }
                }
            }

            return View();
        }
        #endregion

        [Authorize]
        public async Task<IActionResult> DangXuat()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/");
        }
    }
}
