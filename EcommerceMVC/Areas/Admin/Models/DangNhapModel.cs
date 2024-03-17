using System.ComponentModel.DataAnnotations;

namespace EcommerceMVC.Areas.Admin.Models
{
    public class DangNhapModel
    {
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Chưa nhập email")]
        [EmailAddress(ErrorMessage = "Chưa đúng định dạng email")]
        public string Email { get; set; }

        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "Chưa nhập mật khẩu")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
