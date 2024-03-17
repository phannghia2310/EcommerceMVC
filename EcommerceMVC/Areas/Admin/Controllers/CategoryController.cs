using AutoMapper;
using EcommerceMVC.Areas.Admin.Models;
using EcommerceMVC.Data;
using EcommerceMVC.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(AuthenticationSchemes = "AdminAuth", Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly Ecommerce2024Context _context;
        private readonly IMapper _mapper;

        public CategoryController(Ecommerce2024Context context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            var loais = _context.Loais.AsQueryable();

            var result = loais.Select(l => new LoaiModel
            {
                MaLoai = l.MaLoai,
                TenLoai = l.TenLoai,
                TenLoaiAlias = l.TenLoaiAlias,
                MoTa = l.MoTa,
                Hinh = l.Hinh
            });

            return View(result);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(LoaiModel model, IFormFile Hinh)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var check = _context.Loais.FirstOrDefault(l => l.MaLoai == model.MaLoai);
                    if (check == null)
                    {
                        var loai = _mapper.Map<Loai>(model);

                        if (Hinh != null)
                        {
                            loai.Hinh = MyUtil.UploadHinh(Hinh, "Loai");
                        }

                        _context.Loais.Add(loai);
                        await _context.SaveChangesAsync();

                        return RedirectToAction("Index", "Category");
                    }
                    else
                    {
                        ViewBag.ErrorRegister = "Loại sản phẩm đã tồn tại";
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
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loai = _context.Loais.FirstOrDefault(l => l.MaLoai == id);
            if (loai == null)
            {
                return NotFound();
            }

            var result = new LoaiModel
            {
                MaLoai = loai.MaLoai,
                TenLoai = loai.TenLoai,
                TenLoaiAlias = loai.TenLoaiAlias,
                MoTa = loai.MoTa,
                Hinh = loai.Hinh
            };

            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(LoaiModel model, IFormFile Hinh)
        {
            if (model == null)
            {
                return NotFound();
            }

            var loai = _context.Loais.AsNoTracking().FirstOrDefault(l => l.MaLoai == model.MaLoai);

            if (loai != null)
            {
                loai = _mapper.Map<Loai>(model);

                if (Hinh != null)
                {
                    loai.Hinh = MyUtil.UploadHinh(Hinh, "NhaCC");
                }

                _context.Loais.Update(loai);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Category");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var loai = _context.Loais.FirstOrDefault(l => l.MaLoai == id);

            if (loai == null)
            {
                return NotFound();
            }

            _context.Loais.Remove(loai);
            await _context.SaveChangesAsync();


            return RedirectToAction("Index", "Category");
        }
    }
}
