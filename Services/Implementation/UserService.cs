using WebApplication1.DTOS;
using WebApplication1.Entitys;
using WebApplication1.Models;
using Microsoft.AspNetCore.Identity;
using WebApplication1.Services.Interface;
using WebApplication1.Repository.Interface;

namespace WebApplication1.Services.Implementation
{
    public class UserService : IUserService 
    {
        private readonly IUserRepository _userRepository;

        private readonly PasswordHasher<string> _passwordHasher = new PasswordHasher<string>();

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<List<UserModel>> GetUsers()
        {
            var users = await _userRepository.GetUsersAsync();
            return users.Select(x => new UserModel
            {
                UserId = x.UserId,
                UserName = x.UserName,
                UserEmail = x.UserEmail,
                AddressId = x.AddressId,

            }).ToList();
        }
        public async Task<UserModel?> GetUserById(int id)
        {
            var Result =await _userRepository.GetByIdAsync(id);
            if (Result == null) {return null;}
            return new UserModel
            {
                UserId = Result.UserId,
                UserName = Result.UserName,
                UserEmail = Result.UserEmail,
                AddressId = Result.AddressId,
            };
        }

        
        public async Task<UserDTO?> UpdateUser(int id, User user)
        {
            var Result = await _userRepository.UpdateByIdAsync(id, user);
            if (Result == null) return null;

            return new UserDTO
            {
                UserName = Result.UserName,
                UserEmail = Result.UserEmail,
                UserPassword = Result.UserPassword,
                AddressId = Result.AddressId,
            };
        }


        public async Task<bool?> DeleteUser(int id)
        {
            var Result = await _userRepository.DeleteByIdAsync(id);
            if (Result == false) { return false; }
            return true;
        }



    }
}
