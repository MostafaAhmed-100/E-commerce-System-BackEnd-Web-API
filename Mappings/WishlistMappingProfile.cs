using AutoMapper;
using WebApplication1.Entitys;
using WebApplication1.DTOS.Response_DTOs;

namespace WebApplication1.Mappings
{
    public class WishlistMappingProfile : Profile
    {
        public WishlistMappingProfile()
        {
            CreateMap<Wishlist, WishlistDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.WishlistId))
                .ForMember(dest => dest.ListName, opt => opt.MapFrom(src => src.WishlistName))
                .ForMember(dest => dest.ItemsCount, opt => opt.MapFrom(src => src.Items != null ? src.Items.Count : 0));

            CreateMap<WishlistItem, WishlistItemResponseDto>()
                .ForMember(dest => dest.ItemId, opt => opt.MapFrom(src => src.WishlistItemId))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.productVariant.Product.ProductName))
                .ForMember(dest => dest.ProductPrice, opt => opt.MapFrom(src => src.productVariant.Price));
        }
    }
}