using EcommerceMVC.Data;
using System.ComponentModel.DataAnnotations;

namespace EcommerceMVC.Areas.Admin.Models
{
    public class HoaDonModel
    {
        [Key]
        [Display(Name = "Mã hóa đơn")]
        public int MaHd { get; set; }
        
        [Display(Name = "Ngày đặt")]                        
        public DateTime? NgayDat { get; set; }

        [Display(Name = "Ngày giao (dự kiến)")]
        public DateTime? NgayGiao { get; set; }

        [Display(Name = "Phương thức thanh toán")]
        public string CachThanhToan { get; set; }

        [Display(Name = "Trạng thái")]
        public int MaTrangThai { get; set; }

        public virtual TrangThai? MaTrangThaiNavigation { get; set; }
    }
}
