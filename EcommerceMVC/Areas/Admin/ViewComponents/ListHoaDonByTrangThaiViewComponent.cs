using EcommerceMVC.Areas.Admin.Models;
using EcommerceMVC.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceMVC.Areas.Admin.ViewComponents
{
    public class ListHoaDonByTrangThaiViewComponent : ViewComponent
    {
        private readonly Ecommerce2024Context _context;

        public ListHoaDonByTrangThaiViewComponent(Ecommerce2024Context context) => _context = context;

        public async Task<IViewComponentResult> InvokeAsync(int status)
        {
            var hoaDons = await _context.HoaDons.AsNoTracking().Where(hd => hd.MaTrangThai == status).ToListAsync();

            var result = hoaDons.Select(hd => new HoaDonModel
            {
                MaHd = hd.MaHd,
                NgayDat = hd.NgayDat,
                NgayGiao = hd.NgayGiao,
                CachThanhToan = hd.CachThanhToan,
                MaTrangThai = hd.MaTrangThai,
                MaTrangThaiNavigation = hd.MaTrangThaiNavigation
            }).ToList();

            return View(result);
        }
    }
}
