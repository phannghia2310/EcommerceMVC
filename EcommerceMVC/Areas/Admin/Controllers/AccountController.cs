using EcommerceMVC.Areas.Admin.Models;
using EcommerceMVC.Data;
using EcommerceMVC.Helpers;
using EcommerceMVC.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace EcommerceMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        private readonly Ecommerce2024Context _context;

        public AccountController(Ecommerce2024Context context) => _context   = context;

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(DangNhapModel model)
        {
            if(ModelState.IsValid)
            {
                var nhanVien = _context.NhanViens.SingleOrDefault(nv => nv.Email == model.Email);
                if (nhanVien == null)
                {
                    ModelState.AddModelError("loi", "Không tồn tại nhân viên này.");
                }
                else
                {
                    if (nhanVien.MatKhau != model.Password.ToMd5Hash(MySetting.PASS_KEY))
                    {
                        ModelState.AddModelError("loi", "Sai mật khẩu.");
                    }
                    else
                    {
                        if (nhanVien.MaPb == "BGD")
                        {
                            var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Email, nhanVien.Email),
                                new Claim(ClaimTypes.Name, nhanVien.HoTen),
                                new Claim(MySetting.CLAIM_EMPLOYEEID, nhanVien.MaNv),

                                //claim - role động
                                new Claim(ClaimTypes.Role, "Admin"),
                            };

                            var scheme = HttpContext.Request.Path.StartsWithSegments("/Admin") ? "AdminAuth" : "CustomerAuth";
                            var claimsIdentity = new ClaimsIdentity(claims, scheme);
                            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                            await HttpContext.SignInAsync(scheme, claimsPrincipal);

                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Email, nhanVien.Email),
                                new Claim(ClaimTypes.Name, nhanVien.HoTen),
                                new Claim(MySetting.CLAIM_EMPLOYEEID, nhanVien.MaNv),

                                //claim - role động
                                new Claim(ClaimTypes.Role, "Employee"),
                            };

                            var scheme = HttpContext.Request.Path.StartsWithSegments("/Admin") ? "AdminAuth" : "CustomerAuth";
                            var claimsIdentity = new ClaimsIdentity(claims, scheme);
                            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                            await HttpContext.SignInAsync(scheme, claimsPrincipal);
                            return RedirectToAction("Index", "Home");
                        }
                    }
                }
            }
            return View();
        }

        [Authorize(AuthenticationSchemes = "AdminAuth")]
        public async Task<IActionResult> Logout()
        {
            var scheme = HttpContext.Request.Path.StartsWithSegments("/Admin") ? "AdminAuth" : "CustomerAuth";
            await HttpContext.SignOutAsync(scheme);
            return RedirectToAction("Login", "Account");
        }
    }
}
