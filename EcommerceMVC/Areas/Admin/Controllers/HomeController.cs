using EcommerceMVC.Areas.Admin.Models;
using EcommerceMVC.Data;
using EcommerceMVC.Helpers;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(AuthenticationSchemes = "AdminAuth")]
    public class HomeController : Controller
    {
        private readonly Ecommerce2024Context _context;

        public HomeController(Ecommerce2024Context context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var Month = DateTime.Now.Month;

            //Tổng đơn hàng trong tháng
            var DonHang = _context.HoaDons.Where(dh => dh.NgayDat.Month == Month).Count();
            //Tổng khách hàng đăng ký
            var KhachHang = _context.KhachHangs.Count();
            //Tổng doanh thu trong tháng
            var hoaDons = _context.HoaDons.Where(dh => dh.NgayGiao.Value.Month == Month).ToList();
            var DoanhThu = 0.0d;
            foreach(var hd in hoaDons)
            {
                var cthds = _context.ChiTietHds.Where(ct => ct.MaHd == hd.MaHd).ToList();
                foreach (var cthd in cthds)
                {
                    DoanhThu += (cthd.DonGia * cthd.SoLuong) - (cthd.DonGia * cthd.GiamGia);
                }
            }
            //Tổng hàng hóa
            var HangHoa = _context.HangHoas.Count();

            var data = new ThongKe
            {
                tongDonHang = DonHang,
                tongKhachHang = KhachHang,
                tongDoanhThu = DoanhThu.ToString("###,###,###.##"),
                tongHangHoa = HangHoa
            };

            var adminId = HttpContext.User.Claims.SingleOrDefault(p => p.Type == MySetting.CLAIM_EMPLOYEEID).Value;
            var nhanVien = _context.NhanViens.SingleOrDefault(nv => nv.MaNv == adminId);
            ViewBag.EmployeeName = nhanVien.HoTen; 

            return View(data);
        }

        [HttpPost]
        public List<object> DoanhThuTungMatHang()
        {
            List<object> data = new List<object>();

            List<string> labels = _context.ChiTietHds.Include(ct => ct.MaHhNavigation).Select(ct => ct.MaHhNavigation.TenHh).Distinct().ToList();
            data.Add(labels);

            List<double> revenues = _context.ChiTietHds.GroupBy(ct => ct.MaHh).Select(ct => ct.Sum(ct => ct.DonGia*ct.SoLuong)).ToList();
            data.Add(revenues);

            return data;
        }

        [HttpPost]
        public List<object> DoanhThuTheoNam()
        {
            List<object> data = new List<object>();

            List<int> labels = _context.HoaDons
                                            .Where(hd => hd.NgayGiao.HasValue)
                                            .Select(hd => hd.NgayGiao.Value.Year)
                                            .Distinct()
                                            .OrderBy(year => year)
                                            .ToList();
            data.Add(labels);

            List<double> revenues = _context.ChiTietHds
                                        .Include(ct => ct.MaHdNavigation)
                                        .Where(ct => ct.MaHdNavigation.NgayGiao.HasValue)
                                        .GroupBy(ct => ct.MaHdNavigation.NgayGiao.Value.Year)
                                        .Select(g => new
                                        {
                                            Year = g.Key,
                                            Revenue = g.Sum(ct => ct.DonGia * ct.SoLuong) 
                                        })
                                        .OrderBy(x => x.Year)
                                        .Select(x => x.Revenue)
                                        .ToList();
            data.Add(revenues);

            return data;
        }
    }
}
