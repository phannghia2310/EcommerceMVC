using System.ComponentModel.DataAnnotations;

namespace EcommerceMVC.Areas.Admin.Models
{
    public class LoaiModel
    {
        [Display(Name = "Mã loại")]
        public int MaLoai { get; set; }

        [Display(Name = "Tên loại")]
        [Required(ErrorMessage = "*")]
        [MaxLength(50, ErrorMessage = "Tối đa 50 kí tự")]
        public string TenLoai { get; set; }

        [Display(Name = "Tên loại alias")]
        public string? TenLoaiAlias { get; set; }

        [Display(Name = "Mô tả")]
        [MaxLength(50, ErrorMessage = "Tối đa 50 kí tự")]
        public string? MoTa { get; set; }

        [Display(Name = "Hình")]
        public string? Hinh { get; set; }
    }
}
