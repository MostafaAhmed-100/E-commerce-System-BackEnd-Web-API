using Microsoft.AspNetCore.Mvc;
using WebApplication1.DTOS;
using WebApplication1.Models;
using WebApplication1.Services.Interface;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        ResponsModel response = new();

        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        [HttpGet("Get-All")]
        public async Task<IActionResult> GetAll()
        {
            var GetAll = await _productService.GetAllProducts();
            response.Status = true;
            response.Data = GetAll;
            response.StatusMessage="";
            return Ok(response);
        }
        [HttpGet("Get-By-Product-Id/{Id:int}")]
        public async Task<IActionResult> GetById(int Id)
        {
            var Result = await _productService.GetByProductId(Id);
            if (Result == null)
            {
                response.Status = false;
                response.Data = Result;
                response.StatusMessage = "There is no Data Found";
                return Ok(response);
            }
            response.Status = true;
            response.Data = Result;
            response.StatusMessage = "";
            return Ok(response);
        }
        [HttpPost("Create-Product")]
        public async Task<IActionResult> CreateProducts([FromForm] ProductDTO productDTO)
        {
            var AddProduct = await _productService.CreateProduct(productDTO);
            response.Status = true;
            response.Data = AddProduct;
            response.StatusMessage = "The Product Has been Added";
            return Ok(response);
        }
        [HttpPut("Update-Product")]
        public async Task<IActionResult> UpdateProduct(int Id,Product product)
        {
            var Update = await _productService.UpdateProduct(Id , product);
            if (Update == null)
            {
                response.Status = false;
                response.Data = Update;
                response.StatusMessage = "There is no Data Found";
                return Ok(response );
            }
            return Ok(Update);
        }
        [HttpDelete("Delete-Product{Id:int}")]
        public async Task<IActionResult> DeleteProduct(int Id)
        {
            var Result = await _productService.DeleteByProductId(Id);
            if (Result == false)
            {
                response.Status = false;
                response.Data = Result;
                response.StatusMessage = "There is no Data Found";

            }
            response.Status = true;
            response.Data = Result;
            response.StatusMessage = "";
            return Ok(response);
        }

    }
}