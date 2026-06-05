using WebApplication1.DTOS.Request_DTOs;
using WebApplication1.DTOS.Response_DTOs;
using WebApplication1.DTOS.Shared.Response_DTOs;

namespace WebApplication1.Services.OrderService
{
    public interface IOrderService
    {
        Task<ApiResponseDto<OrderResponseDto>> CreateOrderAsync(CreateOrderRequestDto createOrderRequestDto, int buyerId , int userId);
        Task<ApiResponseDto<IEnumerable<OrderResponseDto>>> GetOrdersByBuyerIdAsync(int buyerId, int userId);
        Task<ApiResponseDto<OrderResponseDto>> GetOrderByIdAsync(int orderId, int userId);
        Task<ApiResponseDto<string>> UpdateOrderStatusAsync(int orderId, string newStatus);
        Task<ApiResponseDto<string>> CancelOrderAsync(int orderId, int userId);
    }
}