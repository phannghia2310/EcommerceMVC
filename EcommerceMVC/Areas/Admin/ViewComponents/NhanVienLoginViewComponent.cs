using AutoMapper;
using EcommerceMVC.Areas.Admin.Models;
using EcommerceMVC.Data;
using EcommerceMVC.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceMVC.Areas.Admin.ViewComponents
{
    [Authorize(AuthenticationSchemes = "AdminAuth")]
    public class NhanVienLoginViewComponent : ViewComponent
    {
        private readonly Ecommerce2024Context _context;
        private readonly IMapper _mapper;

        public NhanVienLoginViewComponent(Ecommerce2024Context context, IMapper mapper) 
        {
            _context = context;
            _mapper = mapper;
        }

        public IViewComponentResult Invoke()
        {
            var adminId = HttpContext.User.Claims.SingleOrDefault(p => p.Type == MySetting.CLAIM_EMPLOYEEID)?.Value;
            var nhanVien = _context.NhanViens.SingleOrDefault(nv => nv.MaNv == adminId);
            return View(_mapper.Map<NhanVienModel>(nhanVien));
        }
    }
}
