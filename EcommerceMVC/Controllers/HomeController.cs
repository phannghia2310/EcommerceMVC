using AutoMapper;
using EcommerceMVC.Data;
using EcommerceMVC.Services;
using EcommerceMVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Security.Claims;

namespace EcommerceMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly Ecommerce2024Context _context;
        private readonly IMapper _mapper;

        public HomeController(Ecommerce2024Context context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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

        [HttpGet]
        public IActionResult GetUser()
        {
            var name = "";
            if (User.Identity.IsAuthenticated)
            {
                name = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            }
            else
            {
                var random = new Random();
                name = "Customer" + random.Next(1000, 9999);
            }

            return Ok(new { name = name }); 
        }

        [HttpPost]
        [Route("/Home/SaveMessage")]
        public async Task<IActionResult> SaveMessage([FromBody] MessageVM model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var message = _mapper.Map<Message>(model);
                    _context.Messages.Add(message);
                    await _context.SaveChangesAsync();

                    return Ok();
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Lỗi: {ex.Message}");
                }
            }

            return View(model);
        }

        [Route("/404")]
        public IActionResult PageNotFound()
        {
            return View();
        }
    }
}
