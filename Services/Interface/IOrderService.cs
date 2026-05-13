using WebApplication1.DTOS;
using WebApplication1.Entitys;
using WebApplication1.Models;

namespace WebApplication1.Services.Interface
{
    public interface IOrderService
    {
        public Task<List<OrderModel>> GetAllAsync();
        public Task<OrderModel?> GetByIdAsync(int id);
        public Task<OrderModel> CreateAsync(OrderDTO order);
        public Task<OrderModel?> UpdateByIdAsync(int OrderId, Order order);
        public Task<bool?> DeleteByIdAsync(int id);
    }
}
