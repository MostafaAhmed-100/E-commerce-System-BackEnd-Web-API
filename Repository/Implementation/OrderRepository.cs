using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using WebApplication1.Entitys;
using WebApplication1.Repository.Interface;

namespace WebApplication1.Repository.Implementation
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Order>> GetAll()
        {
            var AllOrders = await _context.Order.ToListAsync();
            return AllOrders;
        }
        public async Task<Order?> GetById(int id)
        {
            var GetByOrderId = await _context.Order.FirstOrDefaultAsync(o => o.OrderId == id);
            return GetByOrderId;
        }
        public async Task<Order> Create(Order order)
        {
            var CreateOrder = await _context.Order.AddAsync(order);
            await _context.SaveChangesAsync();
            return CreateOrder.Entity;
        }
        public async Task<Order?> UpdateById(int Id, Order order)
        {
            var GetByOrderId = await _context.Order.FirstOrDefaultAsync(o => o.OrderId == Id);
            if (GetByOrderId == null) return null;
            GetByOrderId.UserId = order.UserId;
            GetByOrderId.AddressId = order.AddressId;
            GetByOrderId.CartId = order.CartId;
            GetByOrderId.Address = order.Address;
            await _context.SaveChangesAsync();
            return GetByOrderId;
        }
        public async Task<bool?> DeleteById(int Id)
        {
            var GetByOrderId = await _context.Order.FirstOrDefaultAsync(o => o.OrderId == Id);
            if (GetByOrderId == null) return false;
            _context.Order.Remove(GetByOrderId);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
