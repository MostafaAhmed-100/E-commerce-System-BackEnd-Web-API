using WebApplication1.DTOS;
using WebApplication1.Entitys;
using WebApplication1.Models;

namespace WebApplication1.Services.Interface
{
    public interface IUserService
    {
        public Task<List<UserModel>> GetUsers();
        public Task<UserModel?> GetUserById(int id);

        //public Task<UserModel> CreateUser(UserDTO userdto);

        public Task<UserDTO?> UpdateUser(int id ,User user);

        public Task<bool?> DeleteUser(int id);
    }
}
