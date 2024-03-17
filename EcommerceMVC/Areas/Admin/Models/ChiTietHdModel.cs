using System.ComponentModel.DataAnnotations;

namespace EcommerceMVC.Areas.Admin.Models
{
    public class ChiTietHdModel
    {
        [Key]
        [Display(Name = "Mã chi tiết")]
        public int MaCT { get; set; }

        [Display(Name = "Hình")]
        public string? Hinh { get; set; }

        [Display(Name = "Tên hàng hóa")]
        public string? TenHh { get; set; }

        [Display(Name = "Đơn giá")]
        public double DonGia { get; set; }

        [Display(Name = "Số lượng")]
        public int SoLuong { get; set; }

        [Display(Name = "Giảm giá")]
        public double GiamGia { get; set; }

        [Display(Name = "Thành tiền")]
        public double ThanhTien => DonGia * SoLuong;
    }
}
