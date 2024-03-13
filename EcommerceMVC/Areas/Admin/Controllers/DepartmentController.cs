using AutoMapper;
using EcommerceMVC.Areas.Admin.Models;
using EcommerceMVC.Data;
using EcommerceMVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DepartmentController : Controller
    {
        private readonly Ecommerce2024Context _context;
        private readonly IMapper _mapper;

        public DepartmentController(Ecommerce2024Context context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            var phongBans = _context.PhongBans.AsQueryable();

            var result = _context.PhongBans.Select(pb => new PhongBanModel
            {
                MaPb = pb.MaPb,
                TenPb = pb.TenPb,
                ThongTin = pb.ThongTin
            });

            return View(result);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(PhongBanModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var check = _context.PhongBans.FirstOrDefault(pb => pb.MaPb == model.MaPb);
                    if (check == null)
                    {
                        var phongBan = _mapper.Map<PhongBan>(model);

                        _context.PhongBans.Add(phongBan);
                        await _context.SaveChangesAsync();

                        return RedirectToAction("Index", "Department");
                    }
                    else
                    {
                        ViewBag.ErrorRegister = "Phòng ban đã tồn tại";
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
        public IActionResult Edit(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phongBan = _context.PhongBans.FirstOrDefault(pb => pb.MaPb == id);
            if (phongBan == null)
            {
                return NotFound();
            }

            var result = new PhongBanModel
            {
                MaPb = phongBan.MaPb,
                TenPb = phongBan.TenPb,
                ThongTin = phongBan.ThongTin
            };

            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(PhongBanModel model)
        {
            if (model == null)
            {
                return NotFound();
            }

            var phongBan = _context.PhongBans.AsNoTracking().FirstOrDefault(pb => pb.MaPb == model.MaPb);

            if (phongBan != null)
            {
                phongBan = _mapper.Map<PhongBan>(model);

                _context.PhongBans.Update(phongBan);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Department");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var phongBan = _context.PhongBans.FirstOrDefault(pb => pb.MaPb == id);

            if (phongBan == null)
            {
                return NotFound();
            }

            _context.PhongBans.Remove(phongBan);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Department");
        }
    }
}
