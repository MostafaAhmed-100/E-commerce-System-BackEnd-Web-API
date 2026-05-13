using WebApplication1.DTOS;
using WebApplication1.Models;

namespace WebApplication1.Services.Interface
{
    public interface ILoginAndRegisterService
    {
        public Task<LoginAndRegisterModel> Register(LoginAndRegisterDto loginAndRegisterDto);

        public Task<LoginAndRegisterModel?> Login(string email , string password);
    }
}
