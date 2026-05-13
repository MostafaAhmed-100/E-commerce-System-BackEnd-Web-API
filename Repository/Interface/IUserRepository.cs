using WebApplication1.Entitys;

namespace WebApplication1.Repository.Interface
{
    public interface IUserRepository
    {
        public Task<List<User>> GetUsersAsync();
        public Task<User?> GetByIdAsync(int id);
        public Task<User?> UpdateByIdAsync(int id , User user);
        //public Task<User> CreateUser (User user);
        public Task<bool?> DeleteByIdAsync(int id);
    }
}
