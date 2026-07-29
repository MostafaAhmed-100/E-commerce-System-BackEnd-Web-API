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
            var totalpoints = _AppDbcontext.loyaltyTransactions
                .Where(x => x.BuyerId == BuyerId);
            return await totalpoints.SumAsync( x => x.Points);
        }

        public async Task<LoyaltyTransaction?> GetTransactionByOrderIdAndTypeAsync(int OrderId, string type)
        {
            var transaction =  await _AppDbcontext.loyaltyTransactions
                .FirstOrDefaultAsync(x => x.OrderId == OrderId && x.TransactionType == type);

            return  transaction;
        }

        public async Task<(IEnumerable<LoyaltyTransaction>, int TotalCount)?> GetTransactionsByBuyerIdAsync(int buyerId, int pageNumber, int pageSize)
        {
            var query = _AppDbcontext.loyaltyTransactions
                .AsNoTrackingWithIdentityResolution()
                .Where(x => x.BuyerId == buyerId)
                .Include(x => x.Order);

            var TotalCount = await query.CountAsync();
            if (TotalCount == 0)
                return null;
            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToListAsync();

            return (items, TotalCount);
        }
    }
}
