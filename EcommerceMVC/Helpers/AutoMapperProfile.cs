using AutoMapper;
using EcommerceMVC.Areas.Admin.Models;
using EcommerceMVC.Data;
using EcommerceMVC.ViewModels;

namespace EcommerceMVC.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile() 
        {
            CreateMap<DangKyVM, KhachHang>();
                // .ForMember(kh => kh.HoTen, option => option.MapFrom(DangKyVM => DangKyVM.HoTen))
                // .ReverseMap();
            CreateMap<KhachHangVM, KhachHang>().ReverseMap();
            CreateMap<HoaDonVM, HoaDon>();
            CreateMap<ChiTietHdVM, ChiTietHd>().ReverseMap();
            CreateMap<DanhGiaVM, YeuThich>().ReverseMap();
            CreateMap<HoiDapVM, HoiDap>().ReverseMap();

            //Admin
            CreateMap<NhaCungCapModel, NhaCungCap>().ReverseMap();   
            CreateMap<KhachHangModel, KhachHang>().ReverseMap(); 
            CreateMap<PhongBanModel, PhongBan>().ReverseMap();
            CreateMap<NhanVienModel, NhanVien>().ReverseMap();
            CreateMap<LoaiModel, Loai>().ReverseMap();
            CreateMap<HangHoaModel, HangHoa>().ReverseMap();
            CreateMap<HoiDapModel, HoiDap>().ReverseMap();
        }
    }
}
