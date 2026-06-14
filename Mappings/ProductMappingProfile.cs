using AutoMapper;
using WebApplication1.DTOS.Request_DTOs;
using WebApplication1.DTOS.Response_DTOs;
using WebApplication1.Entitys;

namespace WebApplication1.Mappings
{
    public class ProductMappingProfile : Profile
    {
        public ProductMappingProfile() 
        {
            CreateMap<ProductVariant, ProductVariantResponseDto>()
                .ForMember(dest => dest.VariantId,
                opt => opt.MapFrom(src => src.ProductVariantId))
                .ForMember(dest => dest.IsAvailable,
                opt => opt.MapFrom(src => src.QuantityInStock > 0));
            CreateMap<Product, ProductResponseDto>()
                .ForMember(dest => dest.CategoryName,
                opt => opt.MapFrom(src => src.Category.CategoryName != null ? src.Category.CategoryName : "Unknown"))
                .ForMember(dest => dest.SellerStoreName,
                opt => opt.MapFrom(src => src.Seller.StoreName != null ? src.Seller.StoreName : "Unknown"))
                .ForMember(dest => dest.Variants,
                opt => opt.MapFrom(src => src.ProductVariants));

            CreateMap<CreateProductVariantRequestDto, ProductVariant>()
                .ForMember(dest => dest.Color,
                           opt => opt.MapFrom(src => src.Color ?? ""))
                .ForMember(dest => dest.Size,
                           opt => opt.MapFrom(src => src.Size ?? ""))
                .ForMember(dest => dest.Discount,
                           opt => opt.MapFrom(src => 0))

                .ForMember(dest => dest.ReservedQuantity,
                           opt => opt.MapFrom(src => 0));

            CreateMap<CreateProductRequestDto, Product>()
                .ForMember(dest => dest.ProductVariants,
                           opt => opt.MapFrom(src => src.Variants));
        }
    }
}
