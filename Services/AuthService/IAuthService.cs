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
    }
}