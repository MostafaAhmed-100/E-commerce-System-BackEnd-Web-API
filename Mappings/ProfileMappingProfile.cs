using AutoMapper;

namespace WebApplication1.Mappings
{
    public class ProfileMappingProfile : Profile
    {
        public ProfileMappingProfile()
        {
                CreateMap<Entitys.User, DTOS.Response_DTOs.BuyerProfileResponseDto>()
                .ForMember(dest => dest.BuyerName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.BuyerEmail, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.BuyerPhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.Addresses, opt => opt.MapFrom(src => src.Addresses))
                .ForMember(dest => dest.LoyaltyPoints, opt => opt.MapFrom(src => src.Buyer.LoyaltyPoints));
                
            CreateMap<Entitys.Address, DTOS.Shared.Addressdto>()
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
                .ForMember(dest => dest.HomeAddress, opt => opt.MapFrom(src => src.HomeAddress))
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State))
                .ForMember(dest => dest.Zip_Code, opt => opt.MapFrom(src => src.Zip_Code));

            CreateMap<Entitys.Seller, DTOS.Response_DTOs.SellerProfileResponseDto>()
                .ForMember(dest => dest.SellerName, opt => opt.MapFrom(src => src.StoreName))
                .ForMember(dest => dest.SellerEmail, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.SellerPhoneNumber, opt => opt.MapFrom(src => src.User.PhoneNumber))
                .ForMember(dest => dest.SellerNationalId, opt => opt.MapFrom(src => src.NationalId))
                .ForMember(dest => dest.SellerBankName, opt => opt.MapFrom(src => src.BankName))
                .ForMember(dest => dest.SellerBankAccountNumber, opt => opt.MapFrom(src => src.BankAccountNumber));

            CreateMap<DTOS.Request_DTOs.UpdateBuyerProfileRequestDto, Entitys.User>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.BuyerName))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.BuyerPhoneNumber));

            CreateMap<DTOS.Request_DTOs.UpdateSellerProfileRequestDto, Entitys.Seller>()
                .ForMember(dest => dest.StoreName, opt => opt.MapFrom(src => src.SellerStoreName))
                .ForMember(dest => dest.BankName, opt => opt.MapFrom(src => src.SellerBankName))
                .ForMember(dest => dest.BankAccountNumber, opt => opt.MapFrom(src => src.SellerBankAccountNumber));
        }
    }
}
