using System.ComponentModel.DataAnnotations;

namespace EcommerceMVC.Areas.Admin.Models
{
    public class NhaCungCapModel
    {
        [Key]
        [Display(Name = "Mã nhà cung cấp")]
        [Required(ErrorMessage = "*")]
        public string MaNcc { get; set; } = null!;

        [Display(Name = "Tên công ty")]
        [Required(ErrorMessage = "*")]
        [MaxLength(50, ErrorMessage = "Tối đa 50 ký tự")]
        public string TenCongTy { get; set; } = null!;

        [Display(Name = "Logo công ty")]
        public string? Logo { get; set; }

        [Display(Name = "Người liên lạc")]
        [Required(ErrorMessage = "*")]
        [MaxLength(50, ErrorMessage = "Tối đa 50 ký tự")]
        public string? NguoiLienLac { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "*")]
        [EmailAddress(ErrorMessage = "Chưa đúng định dạng email")]
        public string Email { get; set; } = null!;

        [Display(Name = "Số điện thoại")]
        [MaxLength(24, ErrorMessage = "Tối đa 24 ký tự")]
        public string? DienThoai { get; set; }

        [Display(Name = "Địa chỉ")]
        [MaxLength(50, ErrorMessage = "Tối đa 50 ký tự")]
        public string? DiaChi { get; set; }

        [Display(Name = "Mô tả")]
        [MaxLength(50, ErrorMessage = "Tối đa 50 ký tự")]
        public string? MoTa { get; set; }
    }
}
