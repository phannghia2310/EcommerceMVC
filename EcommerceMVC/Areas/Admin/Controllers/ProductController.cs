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
    public class ProductController : Controller
    {
        private readonly Ecommerce2024Context _context;
        private readonly IMapper _mapper;

        public ProductController(Ecommerce2024Context context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            var hangHoas = _context.HangHoas.AsQueryable();

            var result = hangHoas.Select(hh => new HangHoaModel
            {
                MaHh = hh.MaHh,
                TenHh = hh.TenHh,
                MaLoai = hh.MaLoai,
                MoTaDonVi = hh.MoTaDonVi,
                DonGia = hh.DonGia,
                Hinh = hh.Hinh,
                NgaySx = hh.NgaySx,
                GiamGia = hh.GiamGia,
                SoLuongTon = hh.SoLuongTon,
                MoTa = hh.MoTa,
                MaNcc = hh.MaNcc,
                MaLoaiNavigation = hh.MaLoaiNavigation,
                MaNccNavigation = hh.MaNccNavigation,
            }); 

            return View(result);
        }

        [HttpGet]
        public IActionResult Create()
        {
            HangHoaModel model = new HangHoaModel();
            model.listLoai = _context.Loais.ToList();
            model.listNcc = _context.NhaCungCaps.ToList();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(HangHoaModel model, IFormFile Hinh)
        {
            model.listLoai = _context.Loais.ToList();
            model.listNcc = _context.NhaCungCaps.ToList();

            if (ModelState.IsValid)
            {
                try
                {
                    var check = _context.HangHoas.FirstOrDefault(hh => hh.MaHh == model.MaHh);
                    if (check == null)
                    { 
                        var hangHoa = _mapper.Map<HangHoa>(model);

                        if (Hinh != null)
                        {
                            hangHoa.Hinh = MyUtil.UploadHinh(Hinh, "HangHoa");
                        }

                        _context.HangHoas.Add(hangHoa);
                        await _context.SaveChangesAsync();

                        return RedirectToAction("Index", "Product");
                    }
                    else
                    {
                        ViewBag.ErrorRegister = "Hàng hóa đã tồn tại";
                        return View(model);
                    }
                }
                catch (Exception ex)
                {
                    var mes = $"{ex.Message}";
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hangHoa = _context.HangHoas.FirstOrDefault(hh => hh.MaHh == id);
            if (hangHoa == null)
            {
                return NotFound();
            }

            var result = new HangHoaModel
            {
                MaHh = hangHoa.MaHh,
                TenHh = hangHoa.TenHh,
                MaLoai = hangHoa.MaLoai,
                MoTaDonVi = hangHoa.MoTaDonVi,
                DonGia = hangHoa.DonGia,
                Hinh = hangHoa.Hinh,
                NgaySx = hangHoa.NgaySx,
                GiamGia = hangHoa.GiamGia,
                SoLuongTon = hangHoa.SoLuongTon,
                MoTa = hangHoa.MoTa,
                MaNcc = hangHoa.MaNcc,
                MaLoaiNavigation = hangHoa.MaLoaiNavigation,
                MaNccNavigation = hangHoa.MaNccNavigation,
                listLoai = _context.Loais.ToList(),
                listNcc = _context.NhaCungCaps.ToList(),
            };

            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(HangHoaModel model, IFormFile Hinh)
        {
            model.listLoai = _context.Loais.ToList();
            model.listNcc = _context.NhaCungCaps.ToList();

            if (model == null)
            {
                return NotFound();
            }

            var hangHoa = _context.HangHoas.AsNoTracking().FirstOrDefault(hh => hh.MaHh == model.MaHh);

            if (hangHoa != null)
            {
                hangHoa = _mapper.Map<HangHoa>(model);

                if (Hinh != null)
                {
                    hangHoa.Hinh = MyUtil.UploadHinh(Hinh, "HangHoa");
                }

                _context.HangHoas.Update(hangHoa);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Product");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var hangHoa = _context.HangHoas.FirstOrDefault(hh => hh.MaHh == id);

            if (hangHoa == null)
            {
                return NotFound();
            }

            _context.HangHoas.Remove(hangHoa);
            await _context.SaveChangesAsync();


            return RedirectToAction("Index", "Product");
        }
    }
}
