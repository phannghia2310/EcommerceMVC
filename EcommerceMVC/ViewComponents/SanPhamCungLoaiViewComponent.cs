using EcommerceMVC.Data;
using EcommerceMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceMVC.ViewComponents
{
    public class SanPhamCungLoaiViewComponent : ViewComponent
    {
        private readonly Ecommerce2024Context _context;

        public SanPhamCungLoaiViewComponent(Ecommerce2024Context context) => _context = context;

        public IViewComponentResult Invoke(string TenLoai)
        {
            var data = _context.HangHoas
                .Where(hh => hh.MaLoaiNavigation.TenLoai == TenLoai)
                .Select(hh => new ChiTietHangHoaVM
                {
                    MaHangHoa = hh.MaHh,
                    Hinh = hh.Hinh,
                    TenHangHoa = hh.TenHh,
                    TenLoai = hh.MaLoaiNavigation.TenLoai,
                    MoTaNgan = hh.MoTaDonVi,
                    DonGia = hh.DonGia
                });

            return View(data);
        }
    }
}
