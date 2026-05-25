using System.Net.Http.Headers;
using WebApplication1.DTOS;
using WebApplication1.Entitys;
using WebApplication1.Models;
using WebApplication1.Repository.Interface;
using WebApplication1.Services.Interface;

namespace WebApplication1.Services.Implementation
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<List<OrderModel>> GetAllAsync()
        {
            var AllOrder = await _orderRepository.GetAll();
            return AllOrder.Select(o => new OrderModel
            {
                AddressId = o.AddressId,
                OrderId = o.OrderId,
                UserId = o.UserId,
            }).ToList();
        }
        public async Task<OrderModel?> GetByIdAsync(int id)
        {
            var OrderId = await _orderRepository.GetById(id);
            if (OrderId == null) { return null; }

            return new OrderModel
            {
                OrderId = OrderId.OrderId,
                AddressId = OrderId.AddressId,
                UserId = OrderId.UserId,
            };
                
        }
        public async Task<OrderModel> CreateAsync(OrderDTO orderDTO)
        {
            var order = new Order
            {
                AddressId = orderDTO.AddressId,
                CartId = orderDTO.CartId,
                UserId = orderDTO.UserId,
            };
            var CreateOrder = await _orderRepository.Create(order);
            return new OrderModel
            {
                OrderId = CreateOrder.OrderId,
                AddressId = orderDTO.AddressId,
                UserId = orderDTO.UserId,
            };
        }
        public async Task<OrderModel?> UpdateByIdAsync(int OrderId, Order order)
        {
            var Result = await _orderRepository.UpdateById(OrderId, order);
            if (Result == null) { return null; };
            return new OrderModel
            {
                UserId = Result.UserId,
                OrderId = Result.OrderId,
                AddressId = Result.AddressId,
            };
        }
        public async Task<bool?> DeleteByIdAsync(int id)
        {
            var DeleteOrder = await _orderRepository.DeleteById(id);
            return DeleteOrder;
        }

    }
}
