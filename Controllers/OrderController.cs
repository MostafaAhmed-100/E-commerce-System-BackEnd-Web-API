using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.DTOS;
using WebApplication1.Entities;
using WebApplication1.Entitys;
using WebApplication1.Models;
using WebApplication1.Services.Interface;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        ResponsModel response = new();
        IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        [HttpGet("Get-All")]
        public async Task<IActionResult> GetAll()
        {
            var Orders = await _orderService.GetAllAsync();
            response.Status = true;
            response.Data = Orders;
            return Ok(response);

        }
        [HttpGet("Get-By-Id")]
        public async Task<IActionResult> GetById(int id)
        {
            var OrderById = await _orderService.GetByIdAsync(id);
            response.Status = true;
            response.Data = OrderById;
            return Ok(response);
        }
        [HttpPost("Create-Order")]
        public async Task<IActionResult> CreateOrder (OrderDTO orderDTO)
        {
            var Create = await _orderService.CreateAsync(orderDTO);
            response.Status = true;
            response.Data = Create;
            return Ok(response);
        }
        [HttpPut("Update-Order")]
        public async Task<IActionResult> UpdateOrder(int id , Order order)
        {
            var Result = await _orderService.UpdateByIdAsync(id, order);
            if (Result == null)
            {
                response.Status = false;
                response.Data = Result;
                response.StatusMessage = "No Data Was Found";
            }
            else
            {
                response.Status = true;
                response.Data = Result;

            }
            return Ok(response);
        }
        [HttpDelete("Delete-By-Id{Id}")]
        public async Task<IActionResult> DeleteById(int Id)
        {
            var Deleted = await _orderService.DeleteByIdAsync(Id);
            if (Deleted == null)
            {
                response.Status = false;
                response.StatusMessage = "No Data Was Found";
            }
            else
            {
                response.Status = true;
                response.Data = Deleted;
            }
            return Ok(response);
        }
    }
}
