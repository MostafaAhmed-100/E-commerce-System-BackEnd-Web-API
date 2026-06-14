using AutoMapper;
using WebApplication1.DTOS.Request_DTOs;
using WebApplication1.DTOS.Response_DTOs;
using WebApplication1.Entitys;

namespace WebApplication1.Mappings
{
    public class CouponMappingProfile : Profile
    {
        public CouponMappingProfile() 
        {
            CreateMap<Coupon, CouponResponseDto>();
            CreateMap<CreateCouponRequestDto, Coupon>();
            CreateMap<UpdateCouponRequestDto, Coupon>();
        }
    }
}
