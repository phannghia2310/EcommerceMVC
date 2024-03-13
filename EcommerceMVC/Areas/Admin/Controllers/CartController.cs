using Microsoft.AspNetCore.Mvc;

namespace EcommerceMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
