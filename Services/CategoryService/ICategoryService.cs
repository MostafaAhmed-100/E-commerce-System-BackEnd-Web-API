using WebApplication1.DTOS.Request_DTOs;
using WebApplication1.DTOS.Response_DTOs;
using WebApplication1.DTOS.Shared.RequestDto;
using WebApplication1.DTOS.Shared.Response_DTOs;

namespace WebApplication1.Services.CategoryService
{
    public interface ICategoryService
    {
        Task<ApiResponseDto<CategoryResponseDto>> CreateCategoryAsync(CreateCategoryRequestDto createCategoryRequestDto);

        Task<ApiResponseDto<CategoryResponseDto>> UpdateCategoryAsync(CreateCategoryRequestDto createCategoryRequestDto, int categoryId);

        Task <ApiResponseDto<string>> DeleteCategoryAsync(int categoryId);

        Task <ApiResponseDto<CategoryResponseDto>> GetCategoryByIdAsync(int categoryId);

        Task<ApiResponseDto<PaginatedResponseDto<CategoryResponseDto>>> GetAllCategoriesAsync(PaginationRequestDto paginationRequestDto);
    }
}
