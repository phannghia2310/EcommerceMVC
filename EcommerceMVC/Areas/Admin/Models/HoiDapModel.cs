using EcommerceMVC.Data;
using System.ComponentModel.DataAnnotations;

namespace EcommerceMVC.Areas.Admin.Models
{
    public class HoiDapModel
    {
        [Key]
        [Display(Name = "Mã hỏi đáp")]
        public int MaHd { get; set; }

        [Display(Name = "Họ tên")]
        public string? HoTen { get; set; }

        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Chưa đúng định dạng email")]
        public string? Email { get; set; }

        [Display(Name = "Câu hỏi")]
        [MaxLength(100, ErrorMessage = "Tối đa 100 kí tự")]
        public string CauHoi { get; set; }

        [Display(Name = "Trả lời")]
        [MaxLength(100, ErrorMessage = "Tối đa 100 kí tự")]
        public string? TraLoi { get; set; }

        [Display(Name = "Ngày gửi")]
        public DateTime NgayDua { get; set; }

        [Display(Name = "Nhân viên")]
        public string? MaNv { get; set; }

        public virtual NhanVien? MaNvNavigation { get; set; }
    }
}

