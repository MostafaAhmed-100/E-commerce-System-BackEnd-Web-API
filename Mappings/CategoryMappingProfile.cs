using AutoMapper;
using WebApplication1.DTOS.Request_DTOs;
using WebApplication1.DTOS.Request_DTOs.Category;
using WebApplication1.DTOS.Response_DTOs;
using WebApplication1.Entitys;

namespace WebApplication1.Mappings
{
    public class CategoryMappingProfile : Profile
    {
        public CategoryMappingProfile() 
        {
            CreateMap<Category, CategoryResponseDto>();
            CreateMap<CreateCategoryRequestDto ,Category>();
            CreateMap<UpdateCategoryRequestDto ,Category>();
        }
    }
}
