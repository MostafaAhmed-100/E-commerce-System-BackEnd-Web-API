using Microsoft.EntityFrameworkCore;
using WebApplication1.Entitys;
using WebApplication1.Repository.Interface;

namespace WebApplication1.Repository.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _AppDbcontext;

        public UserRepository(AppDbContext appDbcontext)
        {
            _AppDbcontext = appDbcontext;
        }

        public async Task<List<User>> GetUsersAsync()
        {
            var AllUsers = await _AppDbcontext.Users.Include(a => a.Addresses).ToListAsync();
            return AllUsers;
        }
        public async Task<User?> GetByIdAsync(int id)
        {
            var GetById = await _AppDbcontext.Users.Include(a => a.Addresses).FirstOrDefaultAsync(U => U.UserId == id);
            return GetById;
        }
       // public async Task<User> CreateUser(User user)
       // {
       //     var CreateUser = await _AppDbcontext.Users.AddAsync(user);
       //     await _AppDbcontext.SaveChangesAsync();
       //     return user;
       // }

        public async Task<User?> UpdateByIdAsync(int id, User user)
        {
            var Result = await _AppDbcontext.Users.FirstOrDefaultAsync(u => u.UserId == id);
            if (Result == null) {return null;}
            Result.UserEmail = user.UserEmail;
            Result.UserName = user.UserName;
            Result.UserPassword = user.UserPassword;
            Result.AddressId = user.AddressId;
            await _AppDbcontext.SaveChangesAsync();
            return Result;
        }
        public async Task<bool?> DeleteByIdAsync(int id)
        {
            var Result = await _AppDbcontext.Users.Include(a => a.Addresses).FirstOrDefaultAsync(u => u.UserId == id);
            if (Result == null) { return false; }
            _AppDbcontext.Remove(Result);
            await _AppDbcontext.SaveChangesAsync();
            return true;

        }
    }
}
