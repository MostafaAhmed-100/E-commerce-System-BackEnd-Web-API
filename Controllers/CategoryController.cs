using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using WebApplication1.Constants;
using WebApplication1.DTOS.Request_DTOs;
using WebApplication1.DTOS.Request_DTOs.Category;
using WebApplication1.DTOS.Shared.RequestDto;
using WebApplication1.Services.CategoryService;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableRateLimiting("BrowsingPolicy")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpPost("Create-Category")]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequestDto requestDto)
        {
            var result = await _categoryService.CreateCategoryAsync(requestDto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("Update-Category/{categoryId}")]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> UpdateCategory([FromRoute] int categoryId, [FromBody] UpdateCategoryRequestDto requestDto)
        {
            var result = await _categoryService.UpdateCategoryAsync(requestDto, categoryId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("Delete-Category/{categoryId}")]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> DeleteCategory([FromRoute] int categoryId)
        {
            var result = await _categoryService.DeleteCategoryAsync(categoryId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("Get-Category/{categoryId}")]
        public async Task<IActionResult> GetCategoryById([FromRoute] int categoryId)
        {
            var result = await _categoryService.GetCategoryByIdAsync(categoryId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("Get-All")]
        public async Task<IActionResult> GetAllCategories([FromQuery] PaginationRequestDto requestDto)
        {
            var result = await _categoryService.GetAllCategoriesAsync(requestDto);
            return StatusCode(result.StatusCode, result);
        }
    }
}