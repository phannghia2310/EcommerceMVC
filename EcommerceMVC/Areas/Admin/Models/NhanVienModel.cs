using EcommerceMVC.Data;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace EcommerceMVC.Areas.Admin.Models
{
    public class NhanVienModel
    {
        [Key]
        [Display(Name = "Mã nhân viên")]
        [Required(ErrorMessage = "*")]
        public string MaNv { get; set; }

        [Display(Name = "Mã nhân viên")]
        [Required(ErrorMessage = "*")]
        [MaxLength(50, ErrorMessage = "Tối đa 50 kí tự")]
        public string HoTen { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "*")]
        [EmailAddress(ErrorMessage = "Chưa đúng định dạng email")]
        public string Email { get; set; }

        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "*")]
        [MaxLength(50, ErrorMessage = "Tối đa 50 kí tự")]
        public string? MatKhau { get; set; }

        [Display(Name = "Phòng ban")]
        public string MaPb { get; set; }

        public virtual PhongBan? MaPbNavigation { get; set; }

        public List<PhongBan> listPB = new List<PhongBan>();
    }
}
