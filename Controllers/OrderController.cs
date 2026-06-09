using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using WebApplication1.Constants;
using WebApplication1.DTOS.Request_DTOs;
using WebApplication1.Services.OrderService;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [EnableRateLimiting("CheckoutPolicy")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost("Create-Order")]
        [Authorize(Roles = AppRoles.Buyer)]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequestDto requestDto)
        {
            var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var buyerId = Convert.ToInt32(User.FindFirstValue("ProfileId"));

            var result = await _orderService.CreateOrderAsync(requestDto, buyerId, userId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("My-Orders")]
        [Authorize(Roles = AppRoles.Buyer)]
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var buyerId = Convert.ToInt32(User.FindFirstValue("ProfileId"));

            var result = await _orderService.GetOrdersByBuyerIdAsync(buyerId, userId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("Get-Order/{orderId}")]
        [Authorize(Roles = AppRoles.Buyer)]
        public async Task<IActionResult> GetOrderById([FromRoute] int orderId)
        {
            var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var result = await _orderService.GetOrderByIdAsync(orderId, userId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("Cancel-Order/{orderId}")]
        [Authorize(Roles = AppRoles.Buyer)]
        public async Task<IActionResult> CancelOrder([FromRoute] int orderId)
        {
            var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var result = await _orderService.CancelOrderAsync(orderId, userId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("Update-Status/{orderId}")]
        [Authorize(Roles = AppRoles.Admin + "," + AppRoles.Seller)]
        public async Task<IActionResult> UpdateOrderStatus([FromRoute] int orderId, [FromQuery] string newStatus)
        {
            var result = await _orderService.UpdateOrderStatusAsync(orderId, newStatus);
            return StatusCode(result.StatusCode, result);
        }
    }
}