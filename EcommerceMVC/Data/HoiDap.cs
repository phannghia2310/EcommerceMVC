using System;
using System.Collections.Generic;

namespace EcommerceMVC.Data;

public partial class HoiDap
{
    public int MaHd { get; set; }

    public string? HoTen { get; set; }

    public string? Email { get; set; }

    public string CauHoi { get; set; } = null!;

    public string? TraLoi { get; set; }

    public DateTime NgayDua { get; set; }

    public string? MaNv { get; set; }

    public virtual NhanVien? MaNvNavigation { get; set; }
}
