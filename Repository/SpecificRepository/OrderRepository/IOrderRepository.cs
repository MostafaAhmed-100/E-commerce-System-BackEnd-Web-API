using WebApplication1.Entitys;
using WebApplication1.Repository.GenericRepository;

namespace WebApplication1.Repository.SpecificRepository.OrderRepository
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<Order?> GetOrderWithDetailsAsync(int orderId, int buyerId);

        Task<(IEnumerable<Order> Items, int TotalCount)> GetOrdersListByBuyerIdAsync(int buyerId , int pageSize , int pageNumber);

        Task<Order?> GetOrderWithItemsByIdAsync(int orderId);
        Task<bool> HasBuyerPurchasedVariantAsync(int buyerId, int productVariantId);
    }
}
