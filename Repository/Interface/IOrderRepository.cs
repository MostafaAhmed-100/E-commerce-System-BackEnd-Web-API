using Microsoft.Data.SqlClient;
using WebApplication1.Entitys;

namespace WebApplication1.Repository.Interface
{
    public interface IOrderRepository
    {
        public Task<List<Order>> GetAll();

        public Task<Order?> GetById(int id);

        public Task<Order> Create(Order order);

        public Task<Order?> UpdateById(int Id, Order order);

        public Task<bool?> DeleteById(int Id);
    }
}
