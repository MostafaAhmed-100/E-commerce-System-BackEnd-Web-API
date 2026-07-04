using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Entitys;
using WebApplication1.Repository.GenericRepository;

namespace WebApplication1.Repository.SpecificRepository.OrderRepository
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(AppDbContext appDbcontext) : base(appDbcontext)
        {
        }

        public async Task<Order?> GetOrderWithDetailsAsync(int orderId, int buyerId)
        {
            var order = await _AppDbcontext.Orders
                .Where(o => o.OrderId == orderId && o.BuyerId == buyerId)
                .Include(o => o.Address)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.ProductVariant)
                .ThenInclude(pv => pv.Product)
                .AsSplitQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync();
            return order;
        }
        public async Task<(IEnumerable<Order> Items, int TotalCount)> GetOrdersListByBuyerIdAsync(int buyerId, int pageNumber, int pageSize)
        {
            var query = _AppDbcontext.Orders
                .Where(o => o.BuyerId == buyerId && !o.IsDeleted)
                .Include(o => o.Address)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.ProductVariant)
                .ThenInclude(pv => pv.Product)
                .AsSplitQuery()
                .AsNoTracking();
            return await ApplyPaginationAsync(query, pageNumber, pageSize);
        }

        public async Task<Order?> GetOrderWithItemsByIdAsync(int orderId)
        {
            return await _AppDbcontext.Set<Order>()
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.ProductVariant)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }
    }
}
