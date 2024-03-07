using AutoMapper;
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
        }
    }
}
