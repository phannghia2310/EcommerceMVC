﻿namespace EcommerceMVC.ViewModels
{
    public class HangHoaVM
    {
        public int MaHangHoa { get; set; }
        public string? TenHangHoa { get; set; }
        public string? Hinh {  get; set; }
        public double? DonGia { get; set; }
        public string? MoTaNgan{ get; set; }
        public string? TenLoai { get; set; }
        public int MaLoai { get; set; }
    }

    public class ChiTietHangHoaVM
    {
        public int MaHangHoa { get; set; }
        public string? TenHangHoa { get; set; }
        public string? Hinh { get; set; }
        public double? DonGia { get; set; }
        public string? MoTaNgan { get; set; }
        public string? TenLoai { get; set; }
        public string? ChiTiet { get; set; }
        public int? DiemDanhGia { get; set; }
        public int SoLuongTon { get; set; }

    }
}
