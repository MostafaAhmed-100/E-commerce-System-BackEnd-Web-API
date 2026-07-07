using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Entitys;
using WebApplication1.Repository.GenericRepository;
using WebApplication1.Repository.SpecificRepository.CategoryRepository.Interface;

namespace WebApplication1.Repository.SpecificRepository.RefreshTokenRepository
{
    public class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(AppDbContext appDbcontext) : base(appDbcontext)
        {
        }
        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            return await _AppDbcontext.RefreshTokens.FirstOrDefaultAsync(t => t.Token == token);
        }
    }
}
