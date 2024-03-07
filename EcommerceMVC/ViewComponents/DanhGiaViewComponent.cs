using EcommerceMVC.Data;
using EcommerceMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceMVC.ViewComponents
{
    public class DanhGiaViewComponent : ViewComponent
    {
        private readonly Ecommerce2024Context _context;

        public DanhGiaViewComponent(Ecommerce2024Context context) => _context = context;

        public IViewComponentResult Invoke(int id)
        {
            var result = _context.YeuThiches.Where(yt => yt.MaHh == id).Select(yt => new DanhGiaVM
            {
                Hinh = yt.MaKhNavigation.Hinh,
                TenKh = yt.MaKhNavigation.HoTen,
                NgayDang = yt.NgayChon,
                DiemDanhGia = yt.DiemDanhGia,
                MoTa = yt.MoTa
            });

            return View(result);
        }
    }
}
