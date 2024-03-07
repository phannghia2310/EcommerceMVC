using EcommerceMVC.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;
using System.Net.NetworkInformation;

namespace EcommerceMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEmailSender _emailSender;

        public HomeController(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        public IActionResult Index()
        { 
            return View();
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
