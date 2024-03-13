using AutoMapper;
using EcommerceMVC.Data;
using EcommerceMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceMVC.Controllers
{
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
            return View();
        }

        [HttpPost]
        public IActionResult GuiCauHoi(HoiDapVM model)
        {
            if(ModelState.IsValid)
            {
                var check = _context.HoiDaps.FirstOrDefault(hd => hd.MaHd == model.MaHd);
                if(check == null)
                {
                    var hoiDap = _mapper.Map<HoiDap>(model);

                    _context.HoiDaps.Add(hoiDap);
                    _context.SaveChanges();

                    ViewBag.Question = "Gửi câu hỏi thành công";
                }
                else
                {
                    ViewBag.Question = "Gửi câu hỏi không thành công";
                }
            }
            return RedirectToAction("Index", "HoiDap");
        }
    }
}
