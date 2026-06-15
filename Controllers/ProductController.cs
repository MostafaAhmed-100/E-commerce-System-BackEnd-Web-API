using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using WebApplication1.Constants;
using WebApplication1.DTOS.Request_DTOs;
using WebApplication1.DTOS.Shared.RequestDto;
using WebApplication1.Services.ProductService;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableRateLimiting("BrowsingPolicy")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpPost("Create-Product")]
        [Authorize(Roles = AppRoles.Seller)]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequestDto requestDto)
        {
            var sellerId = Convert.ToInt32(User.FindFirstValue("ProfileId"));

            var result = await _productService.CreateProductAsync(requestDto, sellerId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("Update-Product/{productId}")]
        [Authorize(Roles = AppRoles.Seller)]
        public async Task<IActionResult> UpdateProduct([FromRoute] int productId, [FromBody] UpdateProductRequestDto requestDto)
        {
            var sellerId = Convert.ToInt32(User.FindFirstValue("ProfileId"));

            var result = await _productService.UpdateProductAsync(requestDto, sellerId, productId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("Delete-Product/{productId}")]
        [Authorize(Roles = AppRoles.Seller)]
        public async Task<IActionResult> DeleteProduct([FromRoute] int productId)
        {
            var sellerId = Convert.ToInt32(User.FindFirstValue("ProfileId"));

            var result = await _productService.DeleteProductAsync(sellerId, productId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("Get-Product/{productId}")]
        public async Task<IActionResult> GetProductById([FromRoute] int productId)
        {
            var result = await _productService.GetProductByIdAsync(productId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("Out-Of-Stock")]
        [Authorize(Roles = AppRoles.Seller)]
        public async Task<IActionResult> GetOutOfStockProducts()
        {
            var sellerId = Convert.ToInt32(User.FindFirstValue("ProfileId"));

            var result = await _productService.GetOutOfStockProductsAsync(sellerId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("Get-All")]
        public async Task<IActionResult> GetAllProducts([FromQuery] int? categoryId, [FromQuery] PaginationRequestDto requestDto)
        {
            var result = await _productService.GetAllProductsAsync(categoryId, requestDto);
            return StatusCode(result.StatusCode, result);
        }
    }
}