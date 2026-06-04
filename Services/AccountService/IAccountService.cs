using WebApplication1.DTOS.Response_DTOs;

namespace WebApplication1.Services.AccountService
{
    public interface IAccountService
    {
        Task<ApiResponseDto<string>> DeleteAccountAsync(int userId);
    }
}
