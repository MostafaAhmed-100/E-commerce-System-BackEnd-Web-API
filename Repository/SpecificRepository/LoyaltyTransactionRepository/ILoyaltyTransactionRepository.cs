using WebApplication1.Entitys;
using WebApplication1.Repository.GenericRepository;

namespace WebApplication1.Repository.SpecificRepository.LoyaltyTransactionRepository
{
    public interface ILoyaltyTransactionRepository : IGenericRepository<LoyaltyTransaction>
    {
        Task<IEnumerable<LoyaltyTransaction>> GetTransactionsByBuyerIdAsync(int buyerId, int pageNumber, int pageSize);

        Task<LoyaltyTransaction?> GetTransactionByOrderIdAndTypeAsync(int OrderId, string type);

        Task<int> GetTotalPointsFromLedgerByBuyerIdAsync(int BuyerId);
    }
}
