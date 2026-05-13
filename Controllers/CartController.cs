using Microsoft.AspNetCore.Mvc;
using WebApplication1.DTOS;
using WebApplication1.Models;
using WebApplication1.Services.Interface;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        ResponsModel response = new();
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet("Get-All")]
        public async Task<IActionResult> GetAll()
        {
            var carts = await _cartService.GetAllAsync();
            response.Status = true;
            response.Data = carts;
            response.StatusMessage = "";
            return Ok(response);
        }

        [HttpGet("Get-By-Id/{Id}")]
        public async Task<IActionResult> GetById(int Id)
        {
            var result = await _cartService.GetByIdAsync(Id);
            if (result == null)
            {
                response.Status = false;
                response.Data = result;
                response.StatusMessage = "No Data Was Found";
            }
            else
            {
                response.Status = true;
                response.Data = result;
                response.StatusMessage = "";
            }
            return Ok(response);
        }

        [HttpPost("Create-Cart")]
        public async Task<IActionResult> CreateCart(CartDTO cartDTO)
        {
            var create = await _cartService.CreateAsync(cartDTO);
            response.Status = true;
            response.Data = create;
            response.StatusMessage = "Cart Created Successfully";
            return Ok(response);
        }

        [HttpPut("Update-Cart/{Id}")]
        public async Task<IActionResult> UpdateCart(int Id, CartDTO cart)
        {
            var result = await _cartService.UpdateByIdAsync(Id, cart);
            if (result == null)
            {
                response.Status = false;
                response.Data = result;
                response.StatusMessage = "No Data Was Found";
            }
            else
            {
                response.Status = true;
                response.Data = result;
                response.StatusMessage = "Cart Updated Successfully";
            }
            return Ok(response);
        }

        [HttpDelete("Delete-By-Id/{Id}")]
        public async Task<IActionResult> DeleteById(int Id)
        {
            var deleted = await _cartService.DeleteByIdAsync(Id);
            if (deleted == null || deleted == false)
            {
                response.Status = false;
                response.StatusMessage = "No Data Was Found";
            }
            else
            {
                response.Status = true;
                response.Data = deleted;
                response.StatusMessage = "Cart Deleted Successfully";
            }
            return Ok(response);
        }
    }
}
