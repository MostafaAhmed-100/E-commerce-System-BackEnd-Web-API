using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Entitys;
using WebApplication1.Repository.GenericRepository;

namespace WebApplication1.Repository.SpecificRepository.BuyerRepository
{
    public class BuyerRepository : GenericRepository<Buyer>, IBuyerRepository
    {
        public BuyerRepository(AppDbContext appDbcontext) : base(appDbcontext)
        {
        }

        public async Task<Buyer?> GetBuyerByUserId(int UserId)
        {
            var Buyer = await _AppDbcontext.Buyers.FirstOrDefaultAsync(I => I.UserId == UserId);
            return Buyer;
        }
    }
}
