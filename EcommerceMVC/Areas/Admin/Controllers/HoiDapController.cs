using AutoMapper;
using EcommerceMVC.Areas.Admin.Models;
using EcommerceMVC.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;

namespace EcommerceMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HoiDapController : Controller
    {
        private readonly Ecommerce2024Context _context;
        private readonly IMapper _mapper;

        public HoiDapController(Ecommerce2024Context context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            var hoiDaps = _context.HoiDaps.AsQueryable();

            var result = hoiDaps.Select(hd => new HoiDapModel
            {
                MaHd = hd.MaHd,
                HoTen = hd.HoTen,
                CauHoi = hd.CauHoi,
                TraLoi = hd.TraLoi,
                Email = hd.Email,
                MaNv = hd.MaNv,
                MaNvNavigation = hd.MaNvNavigation
            });

            return View(result);
        }

        [HttpGet]
        public IActionResult Answer(int? id)
        {
            var hoiDap = _context.HoiDaps.FirstOrDefault(hd => hd.MaHd == id);
            if (hoiDap == null)
            {
                return NotFound();
            }

            var result = new HoiDapModel
            {
                MaHd = hoiDap.MaHd,
                HoTen = hoiDap.HoTen,
                Email = hoiDap.Email,
                CauHoi = hoiDap.CauHoi,
                TraLoi = hoiDap.TraLoi,
                MaNv = hoiDap.MaNv,
                MaNvNavigation = hoiDap.MaNvNavigation
            };
            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> Answer(HoiDapModel model)
        {
            if (model == null)
            {
                return NotFound();
            }

            var hoiDap = _context.HoiDaps.AsNoTracking().FirstOrDefault(hd => hd.MaHd== model.MaHd);

            if (hoiDap != null)
            {
                hoiDap = _mapper.Map<HoiDap>(model);
                hoiDap.MaNv = "";

                _context.HoiDaps.Update(hoiDap);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "HoiDap");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var hoiDap = _context.HoiDaps.FirstOrDefault(hd => hd.MaHd == id);

            if (hoiDap == null)
            {
                return NotFound();
            }

            _context.HoiDaps.Remove(hoiDap);
            await _context.SaveChangesAsync();


            return RedirectToAction("Index", "HoiDap");
        }
    }
}
