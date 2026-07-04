using WebApplication1.DTOS.Request_DTOs;
using WebApplication1.DTOS.Response_DTOs;

namespace WebApplication1.Services
{
    public interface ISavedCardService
    {
        Task<ApiResponseDto<string>> AddCardAsync(AddSavedCardRequestDto dto, int userId);
        Task<ApiResponseDto<IEnumerable<SavedCardResponseDto>>> GetMyCardsAsync(int userId);
        Task<ApiResponseDto<string>> DeleteCardAsync(int cardId, int userId);
    }
}