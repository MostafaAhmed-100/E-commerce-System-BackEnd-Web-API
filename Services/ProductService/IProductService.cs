using WebApplication1.DTOS.Request_DTOs;
using WebApplication1.DTOS.Response_DTOs;
using WebApplication1.DTOS.Shared.RequestDto;
using WebApplication1.DTOS.Shared.Response_DTOs;

namespace WebApplication1.Services.ProductService
{
    public interface IProductService
    {
        Task<ApiResponseDto<ProductResponseDto>> CreateProductAsync(CreateProductRequestDto createProductRequestDto ,int SellerId);

        Task<ApiResponseDto<ProductResponseDto>> UpdateProductAsync (UpdateProductRequestDto updateProductRequestDto,int SellerId , int ProductId);

        Task<ApiResponseDto<string>> DeleteProductAsync (int SellerId ,int ProductId);
        
        Task<ApiResponseDto<ProductResponseDto>> GetProductByIdAsync (int ProductId);

        Task<ApiResponseDto<IEnumerable<ProductResponseDto>>> GetOutOfStockProductsAsync(int SellerId);

        Task<ApiResponseDto<PaginatedResponseDto<ProductResponseDto>>> GetAllProductsAsync(int? CategoryId , PaginationRequestDto paginationRequestDto);
    }
}
