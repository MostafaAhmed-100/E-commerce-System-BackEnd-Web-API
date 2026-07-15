using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Entitys;
using WebApplication1.Repository.GenericRepository;

namespace WebApplication1.Repository.SpecificRepository.LoyaltyTransactionRepository
{
    public class LoyaltyTransactionRepository : GenericRepository<LoyaltyTransaction>, ILoyaltyTransactionRepository
    {
        public LoyaltyTransactionRepository(AppDbContext appDbcontext) : base(appDbcontext)
        {
        }

        public async Task<int> GetTotalPointsFromLedgerByBuyerIdAsync(int BuyerId)
        {
            var totalpoints =  _AppDbcontext.loyaltyTransactions
                .Where(x => x.BuyerId == BuyerId)
                .Select(x => x.Points);
            return await totalpoints.SumAsync();
        }

        public async Task<LoyaltyTransaction?> GetTransactionByOrderIdAndTypeAsync(int OrderId, string type)
        {
            var transaction =  await _AppDbcontext.loyaltyTransactions
                .FirstOrDefaultAsync(x => x.OrderId == OrderId && x.TransactionType == type);

            return  transaction;
        }

        public async Task<IEnumerable<LoyaltyTransaction>> GetTransactionsByBuyerIdAsync(int buyerId, int pageNumber, int pageSize)
        {
            var transactions = await _AppDbcontext.loyaltyTransactions
                .AsNoTrackingWithIdentityResolution()
                .Where(x => x.BuyerId == buyerId)
                .Include(x => x.Order)
                .OrderByDescending(x => x.CreatedAt)
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToListAsync();

            return  transactions;
        }
    }
}
