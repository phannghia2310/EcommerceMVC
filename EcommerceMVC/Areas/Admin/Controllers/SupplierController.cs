using AutoMapper;
using EcommerceMVC.Areas.Admin.Models;
using EcommerceMVC.Data;
using EcommerceMVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SupplierController : Controller
    {
        private readonly Ecommerce2024Context _context;
        private readonly IMapper _mapper;

        public SupplierController(Ecommerce2024Context context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            var nhaCungCaps = _context.NhaCungCaps.AsQueryable();

            var result = nhaCungCaps.Select(s => new NhaCungCapModel
            {
                MaNcc = s.MaNcc,
                TenCongTy = s.TenCongTy,
                Logo = s.Logo,
                NguoiLienLac = s.NguoiLienLac,
                Email = s.Email,
                DienThoai = s.DienThoai,
                DiaChi = s.DiaChi,
                MoTa = s.MoTa
            });

            return View(result);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(NhaCungCapModel model, IFormFile Hinh)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    var check = _context.NhaCungCaps.FirstOrDefault(ncc => ncc.MaNcc == model.MaNcc);
                    if (check == null)
                    {
                        var nhaCungcap = _mapper.Map<NhaCungCap>(model);

                        if (Hinh != null)
                        {
                            nhaCungcap.Logo = MyUtil.UploadHinh(Hinh, "NhaCC");
                        }

                        _context.NhaCungCaps.Add(nhaCungcap);
                        await _context.SaveChangesAsync();

                        return RedirectToAction("Index", "Supplier");
                    }
                    else
                    {
                        ViewBag.ErrorRegister = "Nhà cung cấp đã tồn tại";
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

            var nhaCungcap = _context.NhaCungCaps.FirstOrDefault(ncc => ncc.MaNcc == id);
            if (nhaCungcap == null)
            {
                return NotFound();
            }

            var result = new NhaCungCapModel
            {
                MaNcc = nhaCungcap.MaNcc,
                TenCongTy = nhaCungcap.TenCongTy,
                Logo = nhaCungcap.Logo,
                NguoiLienLac = nhaCungcap.NguoiLienLac,
                Email = nhaCungcap.Email,
                DienThoai = nhaCungcap.DienThoai,
                DiaChi = nhaCungcap.DiaChi,
                MoTa = nhaCungcap.MoTa
            };

            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(NhaCungCapModel model, IFormFile Hinh)
        {
            if (model == null)
            {
                return NotFound();
            }

            var nhaCungcap = _context.NhaCungCaps.AsNoTracking().FirstOrDefault(ncc  => ncc.MaNcc == model.MaNcc);

            if(nhaCungcap != null)
            {
                nhaCungcap = _mapper.Map<NhaCungCap>(model);
                
                if(Hinh != null)
                {
                    nhaCungcap.Logo = MyUtil.UploadHinh(Hinh, "NhaCC");
                }

                _context.NhaCungCaps.Update(nhaCungcap);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Supplier");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var nhaCungcap = _context.NhaCungCaps.FirstOrDefault(ncc => ncc.MaNcc == id);

            if(nhaCungcap == null)
            {
                return NotFound();
            }

            _context.NhaCungCaps.Remove(nhaCungcap);
            await _context.SaveChangesAsync();


            return RedirectToAction("Index", "Supplier");
        }
    }
}
