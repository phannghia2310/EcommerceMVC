using EcommerceMVC.Data;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace EcommerceMVC.Areas.Admin.Models
{
    public class HangHoaModel
    {
        [Display(Name = "Mã hàng hóa")]
        public int MaHh { get; set; }

        [Display(Name = "Tên hàng hóa")]
        [Required(ErrorMessage = "*")]
        [MaxLength(50, ErrorMessage = "Tối đa 50 kí tự")]
        public string TenHh { get; set; }

        [Display(Name = "Mã loại")]
        public int MaLoai { get; set; }

        [Display(Name = "Mô tả đơn vị")]
        public string? MoTaDonVi { get; set; }

        [Display(Name = "Đơn giá")]
        public double? DonGia { get; set; }

        [Display(Name = "Hình")]
        public string? Hinh { get; set; }

        [Display(Name = "Ngày sản xuất")]
        public DateTime NgaySx { get; set; }

        [Display(Name = "Giảm giá")]
        public double GiamGia { get; set; }

        [Display(Name = "Số lần xem")]
        public int SoLanXem { get; set; }

        [Display(Name = "Mô tả")]
        public string? MoTa { get; set; }

        [Display(Name = "Mã NCC")]
        public string? MaNcc { get; set; }

        public virtual Loai? MaLoaiNavigation { get; set; }

        public virtual NhaCungCap? MaNccNavigation { get; set; }

        public List<Loai> listLoai = new List<Loai>();

        public List<NhaCungCap> listNcc = new List<NhaCungCap>();
    }
}
