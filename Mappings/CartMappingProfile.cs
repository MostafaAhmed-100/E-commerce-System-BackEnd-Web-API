using AutoMapper;
using WebApplication1.DTOS.Response_DTOs;
using WebApplication1.Entitys;

namespace WebApplication1.Mappings
{
    public class CartMappingProfile : Profile
    {
        public CartMappingProfile()
        {
            CreateMap<CartItem, CartItemResponseDto>()
                .ForMember(dest => dest.VariantId,
                           opt => opt.MapFrom(src => src.ProductVariantId))
                .ForMember(dest => dest.ProductName,
                           opt => opt.MapFrom(src => src.ProductVariant.Product.ProductName))
                .ForMember(dest => dest.Color,
                           opt => opt.MapFrom(src => src.ProductVariant.Color))
                .ForMember(dest => dest.Size,
                           opt => opt.MapFrom(src => src.ProductVariant.Size))
                .ForMember(dest => dest.Price,
                           opt => opt.MapFrom(src => src.ProductVariant.Price))
                .ForMember(dest => dest.Discount,
                           opt => opt.MapFrom(src => src.ProductVariant.Discount))
                .ForMember(dest => dest.SubTotal,
                           opt => opt.MapFrom(src => (src.ProductVariant.Price - src.ProductVariant.Discount) * src.Quantity));
        }
    }
}