using EcommerceMVC.Data;
using EcommerceMVC.Services;
using EcommerceMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;
using System.Net.NetworkInformation;

namespace EcommerceMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly Ecommerce2024Context _context;
        private readonly IEmailSender _emailSender;

        public HomeController(Ecommerce2024Context context, IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }

        public IActionResult Index()
        {
            var loais = _context.Loais.Select(l => new MenuLoaiVM
            {
                MaLoai = l.MaLoai,
                TenLoai = l.TenLoai,
            }).ToList();

            var hangHoas = _context.HangHoas.Select(p => new HangHoaVM
            {
                MaHangHoa = p.MaHh,
                TenHangHoa = p.TenHh,
                Hinh = p.Hinh,
                DonGia = p.DonGia ?? 0,
                MoTaNgan = p.MoTaDonVi ?? "",
                TenLoai = p.MaLoaiNavigation.TenLoai,
                MaLoai = p.MaLoaiNavigation.MaLoai
            }).ToList();

            HomeVM home = new HomeVM();
            home.listLoai = loais;
            home.listHangHoa = hangHoas;

            return View(home);
        }

        public IActionResult CheckLoginStatus()
        {
            if (User.Identity.IsAuthenticated)
            {
                return Content("Người dùng đã đăng nhập.");
            }
            else
            {
                return Content("Người dùng đã đăng xuất.");
            }
        }

        [Route("/404")]
        public IActionResult PageNotFound()
        {
            return View();
        }
    }
}
