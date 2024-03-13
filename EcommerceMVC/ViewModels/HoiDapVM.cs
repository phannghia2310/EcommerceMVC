using System.ComponentModel.DataAnnotations;

namespace EcommerceMVC.ViewModels
{
    public class HoiDapVM
    {
        [Key]
        public int MaHd { get; set; }

        public string? HoTen { get; set; }

        public string? Email { get; set; }

        public string CauHoi { get; set; }

        public string? TraLoi { get; set; }

        public DateTime NgayDua = DateTime.Now;

        public string? MaNv { get; set; }
    }
}
