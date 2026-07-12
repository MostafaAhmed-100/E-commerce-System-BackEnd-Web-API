using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Entitys;
using WebApplication1.Repository.GenericRepository;

namespace WebApplication1.Repository.SpecificRepository.SellerRepository
{
    public class SellerRepository : GenericRepository<Seller>, ISellerRepository
    {
        public SellerRepository(AppDbContext appDbcontext) : base(appDbcontext)
        {
        }

        public async Task<Seller?> GetSellerIdByUserId(int UserId)
        {
            var Seller = await _AppDbcontext.Sellers.FirstOrDefaultAsync(I => I.UserId == UserId);
            return Seller;
        }

        public Task<Seller?> GetSellerWithUserById(int SellerId)
        {
            var seller = _AppDbcontext.Sellers.Include(I => I.User)
                .AsNoTracking()
                .AsSplitQuery()
                .FirstOrDefaultAsync(I => I.SellerId == SellerId);
            return seller;
        }
    }
}
