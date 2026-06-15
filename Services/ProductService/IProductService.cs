using WebApplication1.DTOS.Request_DTOs;
using WebApplication1.DTOS.Response_DTOs;
using WebApplication1.DTOS.Shared.RequestDto;
using WebApplication1.DTOS.Shared.Response_DTOs;

namespace WebApplication1.Services.ProductService
{
    public interface IProductService
    {
        Task<ApiResponseDto<ProductResponseDto>> CreateProductAsync(CreateProductRequestDto createProductRequestDto, int sellerId);
        Task<ApiResponseDto<ProductResponseDto>> UpdateProductAsync(UpdateProductRequestDto updateProductRequestDto, int sellerId, int productId);
        Task<ApiResponseDto<string>> DeleteProductAsync(int sellerId, int productId);
        Task<ApiResponseDto<ProductResponseDto>> GetProductByIdAsync(int productId);
        Task<ApiResponseDto<IEnumerable<ProductResponseDto>>> GetOutOfStockProductsAsync(int sellerId);
        Task<ApiResponseDto<PaginatedResponseDto<ProductResponseDto>>> GetAllProductsAsync(int? categoryId, PaginationRequestDto paginationRequestDto);
    }
}