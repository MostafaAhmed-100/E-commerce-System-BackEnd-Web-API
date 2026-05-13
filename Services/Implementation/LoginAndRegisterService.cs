using Microsoft.AspNetCore.Identity;
using WebApplication1.DTOS;
using WebApplication1.Entitys;
using WebApplication1.Models;
using WebApplication1.Repository.Interface;
using WebApplication1.Services.Interface;

namespace WebApplication1.Services.Implementation
{
    public class LoginAndRegisterService : ILoginAndRegisterService
    {
        private readonly ILoginAndRegisterRepository _loginAndRegisterRepository;
        private readonly PasswordHasher<string> _passwordHasher = new PasswordHasher<string>();

        public LoginAndRegisterService(ILoginAndRegisterRepository loginAndRegisterRepository)
        {
            _loginAndRegisterRepository = loginAndRegisterRepository;
        }
        public async Task<LoginAndRegisterModel> Register(LoginAndRegisterDto loginAndRegisterDto)
        {
            var user = new User
            {
                UserName = loginAndRegisterDto.UserName,
                UserEmail = loginAndRegisterDto.UserEmail,
                UserPassword = _passwordHasher.HashPassword(
                    loginAndRegisterDto.UserEmail + loginAndRegisterDto.UserName, 
                    loginAndRegisterDto.UserPassword                             
                )
            };

            var create = await _loginAndRegisterRepository.CreateUser(user);

            return new LoginAndRegisterModel
            {
                Email = create.UserEmail,
                UserName = create.UserName
            };
        }
        public async Task<LoginAndRegisterModel?> Login(string email, string password)
        {
            var login = await _loginAndRegisterRepository.GetByEmail(email);
            if (login == null) return null;

            var result = _passwordHasher.VerifyHashedPassword(login.UserEmail + login.UserName,login.UserPassword, password);

            if (result == PasswordVerificationResult.Success)
            {
                return new LoginAndRegisterModel
                {
                    Email = login.UserEmail,
                    UserName = login.UserName
                };
            }

            return null; 
        }
    }
}
