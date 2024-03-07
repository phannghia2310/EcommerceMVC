namespace EcommerceMVC.ViewModels
{
    public class ChiTietHdVM
    {
        public int MaCT {  get; set; }
        public string TenHh { get; set; }
        public string Hinh { get; set; }
        public double DonGia { get; set; }
        public int SoLuong { get; set; }
        public double GiamGia {  get; set; }
        public double ThanhTien => (DonGia * SoLuong) - (DonGia * GiamGia);
    }
}
