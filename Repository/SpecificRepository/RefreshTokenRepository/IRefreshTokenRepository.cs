using WebApplication1.Entitys;
using WebApplication1.Repository.GenericRepository;

namespace WebApplication1.Repository.SpecificRepository.RefreshTokenRepository
{
    public interface IRefreshTokenRepository : IGenericRepository<RefreshToken>
    {
        public Task<RefreshToken?> GetByTokenAsync(string token);
    }
}
