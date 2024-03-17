using AutoMapper;
using EcommerceMVC.Areas.Admin.Models;
using EcommerceMVC.Data;
using EcommerceMVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(AuthenticationSchemes = "AdminAuth", Roles = "Admin")]
    public class CartController : Controller
    {
        private readonly Ecommerce2024Context _context;
        private readonly IMapper _mapper;

        public CartController(Ecommerce2024Context context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var trangThais = await _context.TrangThais.ToListAsync();

            var result = trangThais.Select(tt => new TrangThaiModel
            {
                MaTrangThai = tt.MaTrangThai,
                TenTrangThai = tt.TenTrangThai,
                MoTa = tt.MoTa
            });
            return View(result);
        }

        public IActionResult OrderDetails(int? id)
        {
            var listCthd = _context.ChiTietHds.AsQueryable();

            if (id.HasValue)
            {
                ViewBag.MaHd = id;
                listCthd = listCthd.Where(ct => ct.MaHdNavigation.MaHd == id);
            }

            var result = listCthd.Select(ct => new ChiTietHdModel
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

        public IActionResult ChangeStatus(int? id)
        {
            var hoaDon = _context.HoaDons.Find(id);

            if (hoaDon.MaTrangThai == 0)
            {
                hoaDon.MaTrangThai = 1;
            }
            else if (hoaDon.MaTrangThai == 1)
            {
                hoaDon.MaTrangThai = 2;
                hoaDon.NgayGiao = DateTime.Now.AddDays(3);
            }

            _context.Update(hoaDon);
            _context.SaveChanges();

            return RedirectToAction("Index", "Cart");
        }
    }
}
