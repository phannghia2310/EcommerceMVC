using System;
using System.Collections.Generic;

namespace EcommerceMVC.Data;

public partial class PhongBan
{
    public string MaPb { get; set; } = null!;

    public string TenPb { get; set; } = null!;

    public string? ThongTin { get; set; }

    public virtual ICollection<NhanVien> NhanViens { get; set; } = new List<NhanVien>();
}
