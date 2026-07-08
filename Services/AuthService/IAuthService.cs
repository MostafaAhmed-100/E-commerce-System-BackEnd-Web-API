using WebApplication1.DTOS.Request_DTOs;
using WebApplication1.DTOS.Response_DTOs;
using System.Threading.Tasks;

namespace WebApplication1.Services.Interface
{
    public interface IAuthService
    {
        Task<ApiResponseDto<AuthResponseDto>> RegisterAsync(RegisterRequestDto registerRequestDto);

        Task<ApiResponseDto<AuthResponseDto>> LoginAsync(LoginRequestDto loginRequestDto);

        Task<ApiResponseDto<AuthResponseDto>> RegisterAdminAsync (RegisterAdminRequestDto registerAdminRequestDto);

        Task<ApiResponseDto<AuthResponseDto>> RegisterSellerAsync (RegisterSellerRequestDto registerSellerRequestDto);

        Task<ApiResponseDto<AuthResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto refreshTokenRequestDto);

        Task<ApiResponseDto<string>> ConfirmEmailAsync(int userId, string code);

        Task<ApiResponseDto<string>> ForgotPasswordAsync(ForgotPasswordRequestDto forgotPasswordRequestDto);
        
        Task <ApiResponseDto<string>> ResetPasswordAsync(ResetPasswordRequestDto resetPasswordRequestDto);
    }
}