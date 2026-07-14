using WebApplication1.DTOS.Request_DTOs;
using WebApplication1.DTOS.Response_DTOs;

namespace WebApplication1.Services.AccountService
{
    public interface IAccountService
    {
        Task<ApiResponseDto<string>> DeleteAccountAsync(int userId);

        Task<ApiResponseDto<string>> ChangePasswordAsync(ChangePasswordRequestDto changePasswordRequestDto, int userId);
        Task<ApiResponseDto<BuyerProfileResponseDto>> GetBuyerProfileAsync(int buyerId);
        Task<ApiResponseDto<SellerProfileResponseDto>> GetSellerProfileByNationalIdAsync(string nationalId);
        Task<ApiResponseDto<SellerProfileResponseDto>> GetSellerProfileAsync(int sellerId);
        Task<ApiResponseDto<string>> UpdateSellerProfileAsync(int sellerId, UpdateSellerProfileRequestDto updateSellerProfile);
        Task<ApiResponseDto<string>> UpdateBuyerProfileAsync(int buyerId, UpdateBuyerProfileRequestDto updateBuyerProfile);
    }
}
