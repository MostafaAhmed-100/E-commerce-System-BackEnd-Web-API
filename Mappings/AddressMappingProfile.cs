using AutoMapper;
using WebApplication1.DTOS.Request_DTOs;
using WebApplication1.DTOS.Response_DTOs;
using WebApplication1.Entitys;

namespace WebApplication1.Mappings
{
    public class AddressMappingProfile : Profile
    {
        public AddressMappingProfile() 
        {
            CreateMap<Address, AddressResponseDto>()
                .ForMember(dest => dest.ZipCode,
                opt => opt.MapFrom(src => src.Zip_Code));
            CreateMap<CreateAddressRequestDto, Address>()
                .ForMember(dest => dest.Zip_Code,
                opt => opt.MapFrom(src => src.ZipCode));
            CreateMap<UpdateAddressRequestDto , Address>()
                 .ForMember(dest => dest.Zip_Code,
                opt => opt.MapFrom(src => src.ZipCode));

        }
    }
}