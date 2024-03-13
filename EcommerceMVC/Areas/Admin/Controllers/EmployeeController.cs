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
    public class EmployeeController : Controller
    {
        private readonly Ecommerce2024Context _context;
        private readonly IMapper _mapper;
        private readonly string key = "123abc";

        public EmployeeController(Ecommerce2024Context context,IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            var nhanViens = _context.NhanViens.AsQueryable();

            var result = _context.NhanViens.Select(nv => new NhanVienModel
            {
                MaNv = nv.MaNv,
                HoTen = nv.HoTen,
                Email = nv.Email,
                MatKhau = nv.MatKhau,
                MaPbNavigation = nv.MaPbNavigation
            });

            return View(result);
        }

        [HttpGet]
        public IActionResult Create()
        {
            NhanVienModel model = new NhanVienModel();
            model.listPB = _context.PhongBans.ToList();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(NhanVienModel model)
        {
            model.listPB = _context.PhongBans.ToList();

            if (ModelState.IsValid)
            {
                try
                {
                    var check = _context.NhanViens.FirstOrDefault(nv => nv.MaNv == model.MaNv);
                    if (check == null)
                    {
                        var nhanVien = _mapper.Map<NhanVien>(model);
                        nhanVien.MatKhau = model.MatKhau.ToMd5Hash(key);

                        _context.NhanViens.Add(nhanVien);
                        await _context.SaveChangesAsync();

                        return RedirectToAction("Index", "Employee");
                    }
                    else
                    {
                        ViewBag.ErrorRegister = "Nhân viên đã tồn tại";
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

            var nhanVien = _context.NhanViens.FirstOrDefault(nv => nv.MaNv == id);
            if (nhanVien == null)
            {
                return NotFound();
            }

            var result = new NhanVienModel
            {
                MaNv = nhanVien.MaNv,
                HoTen = nhanVien.HoTen,
                Email = nhanVien.Email,
                MatKhau = nhanVien.MatKhau,
                MaPbNavigation = nhanVien.MaPbNavigation,
                listPB = _context.PhongBans.ToList()
            };

            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(NhanVienModel model)
        {
            model.listPB = _context.PhongBans.ToList();

            if (model == null)
            {
                return NotFound();
            }

            var nhanVien = _context.NhanViens.AsNoTracking().FirstOrDefault(nv => nv.MaNv == model.MaNv);

            if (nhanVien != null)
            {
                nhanVien = _mapper.Map<NhanVien>(model);
                nhanVien.MatKhau = model.MatKhau.ToMd5Hash(key);

                _context.NhanViens.Update(nhanVien);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Employee");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var nhanVien = _context.NhanViens.FirstOrDefault(nv => nv.MaNv == id);

            if (nhanVien == null)
            {
                return NotFound();
            }

            _context.NhanViens.Remove(nhanVien);
            await _context.SaveChangesAsync();


            return RedirectToAction("Index", "Employee");
        }
    }
}
