using Azure;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.DTOS;
using WebApplication1.Models;
using WebApplication1.Services.Interface;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        ResponsModel response = new();
        private readonly ICategoryService _IcategoryService;

        public CategoryController(ICategoryService icategoryService)
        {
            _IcategoryService = icategoryService;
        }

        [HttpGet("Get-All")]
        public async Task<IActionResult> GetAll()
        {
            var AllCategorys = await _IcategoryService.GetAllAsync();
            response.Status = true;
            response.Data = AllCategorys;
            response.StatusMessage = "";
            return Ok(response);
        }
        [HttpGet("Get-By-Id{Id}")]
        public async Task<IActionResult>    GetById(int Id)
        {
            var Result = await _IcategoryService.GetByIdAsync(Id);
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
                response.StatusMessage = "";
            }
                return Ok(response);
        }
        [HttpPost("Create-Category")]
        public async Task<IActionResult> CreateCategory(CategoryDTO categoryDTO)
        {
            var Create = await _IcategoryService.CreateAsync(categoryDTO);
            response.Status = true;
            response.Data = Create;
            return Ok(response);
        }
        [HttpPut("Update-Categry")]
        public async Task<IActionResult> UpdateCategry(int Id , Category category)
        {
            var Result =await _IcategoryService.UpdateByIdAsync(Id, category);
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
            var Deleted = await _IcategoryService.DeleteByIdAsync(Id);
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
