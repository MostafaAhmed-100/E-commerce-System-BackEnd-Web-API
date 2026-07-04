using AutoMapper;
using WebApplication1.DTOS.Request_DTOs;
using WebApplication1.DTOS.Response_DTOs;
using WebApplication1.Entitys;

namespace WebApplication1.Mappings
{
    public class SavedCardMappingProfile : Profile
    {
        public SavedCardMappingProfile()
        {
            CreateMap<AddSavedCardRequestDto, SavedCard>();

            CreateMap<SavedCard, SavedCardResponseDto>();
        }
    }
}
