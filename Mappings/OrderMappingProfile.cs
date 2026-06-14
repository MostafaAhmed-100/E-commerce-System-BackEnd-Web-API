using AutoMapper;
using WebApplication1.DTOS.Response_DTOs;
using WebApplication1.Entitys;

namespace WebApplication1.Mappings
{
    public class OrderMappingProfile : Profile
    {
        public OrderMappingProfile()
        {
            CreateMap<OrderItem, OrderItemResponseDto>()
                .ForMember(dest => dest.ProductNameSnapshot,
                           opt => opt.MapFrom(src => src.ProductVariant.Product.ProductName))
                .ForMember(dest => dest.UnitPrice,
                           opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.SubTotal,
                           opt => opt.MapFrom(src => src.Quantity * src.Price));

            CreateMap<Order, OrderResponseDto>()
                .ForMember(dest => dest.OrderDate,
                           opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.AppliedCouponCode,
                           opt => opt.MapFrom(src => src.Coupon != null ? src.Coupon.CouponCode : null))
                .ForMember(dest => dest.DiscountAmount,
                           opt => opt.MapFrom(src => src.DiscountAmount ?? 0))
                .ForMember(dest => dest.ShippingAddress,
                           opt => opt.MapFrom(src => src.Address != null
                               ? $"{src.Address.HomeAddress}, {src.Address.City}, {src.Address.State} {src.Address.Zip_Code}"
                               : "No Address"))
                .ForMember(dest => dest.Items,
                           opt => opt.MapFrom(src => src.OrderItems));
        }
    }
}