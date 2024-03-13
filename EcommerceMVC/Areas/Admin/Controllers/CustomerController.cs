using AutoMapper;
using EcommerceMVC.Areas.Admin.Models;
using EcommerceMVC.Data;
using EcommerceMVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CustomerController : Controller
    {
        private readonly Ecommerce2024Context _context;
        private readonly IMapper _mapper;

        public CustomerController(Ecommerce2024Context context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            var khachHangs = _context.KhachHangs.AsQueryable();

            var result = _context.KhachHangs.Select(kh => new KhachHangModel
            {
                MaKh = kh.MaKh,
                MatKhau = kh.MatKhau,
                HoTen = kh.HoTen,
                GioiTinh = kh.GioiTinh,
                NgaySinh = kh.NgaySinh,
                DiaChi = kh.DiaChi,
                DienThoai = kh.DienThoai,
                Email = kh.Email,
                Hinh = kh.Hinh,
                VaiTro = kh.VaiTro,
                HieuLuc = kh.HieuLuc,
                RandomKey = kh.RandomKey
            });

            return View(result);
        }

        [HttpGet]
        public IActionResult Edit(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khachHang = _context.KhachHangs.FirstOrDefault(kh => kh.MaKh == id);
            if (khachHang == null)
            {
                return NotFound();
            }

            var result = new KhachHangModel
            {
                MaKh = khachHang.MaKh,
                MatKhau = khachHang.MatKhau,
                HoTen = khachHang.HoTen,
                GioiTinh = khachHang.GioiTinh,
                NgaySinh = khachHang.NgaySinh,
                DiaChi = khachHang.DiaChi,
                DienThoai = khachHang.DienThoai,
                Email = khachHang.Email,
                Hinh = khachHang.Hinh,
                HieuLuc = khachHang.HieuLuc,
                VaiTro = khachHang.VaiTro,
                RandomKey = khachHang.RandomKey
            };

            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(KhachHangModel model, IFormFile Hinh)
        {
            if (model == null)
            {
                return NotFound();
            }

            var khachHang = _context.KhachHangs.AsNoTracking().FirstOrDefault(kh => kh.MaKh == model.MaKh);

            if (khachHang != null)
            {
                khachHang = _mapper.Map<KhachHang>(model);

                if (Hinh != null)
                {
                    khachHang.Hinh = MyUtil.UploadHinh(Hinh, "NhaCC");
                }

                _context.KhachHangs.Update(khachHang);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Customer");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var khachHang = _context.KhachHangs.FirstOrDefault(kh => kh.MaKh == id);

            if (khachHang == null)
            {
                return NotFound();
            }

            _context.KhachHangs.Remove(khachHang);
            await _context.SaveChangesAsync();


             return RedirectToAction("Index", "Customer");
        }
    }
}
