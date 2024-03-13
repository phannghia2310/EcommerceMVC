using System.ComponentModel.DataAnnotations;

namespace EcommerceMVC.Areas.Admin.Models
{
    public class PhongBanModel
    {
        [Key]
        [Display(Name = "Mã phòng ban")]
        [Required(ErrorMessage = "*")]
        public string MaPb { get; set; }

        [Display(Name = "Tên phòng ban")]
        [Required(ErrorMessage = "*")]
        [MaxLength(50, ErrorMessage = "Tối đa 50 kí tự")]
        public string TenPb { get; set; }

        [Display(Name = "Thông tin")]
        public string? ThongTin { get; set; }
    }
}
