using EcommerceMVC.Data;
using EcommerceMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceMVC.ViewComponents
{
    public class MenuLoaiViewComponent : ViewComponent
    {
        private readonly Ecommerce2024Context _context;

        public MenuLoaiViewComponent(Ecommerce2024Context context) => _context = context;

        public IViewComponentResult Invoke()
        {
            var data = _context.Loais.Select(lo => new MenuLoaiVM
            {
                MaLoai = lo.MaLoai,
                TenLoai = lo.TenLoai,
                SoLuong = lo.HangHoas.Count
            }).OrderBy(lo => lo.TenLoai);
            
            return View(data);
        }
    }
}
