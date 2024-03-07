using System.ComponentModel.DataAnnotations;

namespace EcommerceMVC.ViewModels
{
    public class KhachHangVM
    {
        [Display(Name = "Tên đăng nhập")]
        [Required(ErrorMessage = "*")]
        [MaxLength(20, ErrorMessage = "Tối đa 20 ký tự")]
        public string MaKh { get; set; }

        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "*")]
        [DataType(DataType.Password)]
        public string MatKhau { get; set; }

        [Display(Name = "Họ tên")]
        [Required(ErrorMessage = "*")]
        [MaxLength(50, ErrorMessage = "Tối đa 50 ký tự")]
        public string HoTen { get; set; }

        [Display(Name = "Giới tính")]
        public bool GioiTinh { get; set; } = true;

        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)]
        public DateTime? NgaySinh { get; set; }

        [Display(Name = "Địa chỉ")]
        [MaxLength(60, ErrorMessage = "Tối đa 60 ký tự")]
        public string DiaChi { get; set; }

        [Display(Name = "Số điện thoại")]
        [MaxLength(24, ErrorMessage = "Tối đa 24 ký tự")]
        [RegularExpression(@"^(0[3|5|7|8|9])+([0-9]{8})\b", ErrorMessage = "Chưa đúng định dạng số điện thoại Việt Nam")]
        public string DienThoai { get; set; }

        [EmailAddress(ErrorMessage = "Chưa đúng định dạng email")]
        public string Email { get; set; } = null!;

        [Display(Name = "Hình")]
        public string? Hinh { get; set; }
    }
}
