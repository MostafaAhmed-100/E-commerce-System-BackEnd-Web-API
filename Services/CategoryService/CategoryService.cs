using WebApplication1.DTOS.Request_DTOs;
using WebApplication1.DTOS.Response_DTOs;
using WebApplication1.DTOS.Shared.RequestDto;
using WebApplication1.DTOS.Shared.Response_DTOs;
using WebApplication1.Entitys;
using WebApplication1.Repository.GenericRepository;
using WebApplication1.Repository.SpecificRepository.CartRepository;
using WebApplication1.Repository.SpecificRepository.CategoryRepository.Interface;

namespace WebApplication1.Services.CategoryService
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<ApiResponseDto<CategoryResponseDto>> CreateCategoryAsync(CreateCategoryRequestDto dto)
        {
            var allCategories = await _categoryRepository.GetAllAsync();

            if (allCategories.Any(c => c.CategoryName.ToLower() == dto.CategoryName.ToLower()))
            {
                return new ApiResponseDto<CategoryResponseDto>
                {
                    IsSuccess = false,
                    StatusCode = 409,
                    ErrorCode = "CATEGORY_NAME_EXISTS",
                    Message = "A category with this name already exists.",
                    Data = null
                };
            }

            if (dto.ParentCategoryId.HasValue)
            {
                var parent = await _categoryRepository.GetByIdAsync(dto.ParentCategoryId.Value);
                if (parent == null)
                {
                    return new ApiResponseDto<CategoryResponseDto>
                    {
                        IsSuccess = false,
                        StatusCode = 400,
                        ErrorCode = "PARENT_CATEGORY_NOT_FOUND",
                        Message = "The specified parent category does not exist.",
                        Data = null
                    };
                }
            }

            var category = new Category
            {
                CategoryName = dto.CategoryName,
                ParentCategoryId = dto.ParentCategoryId
            };

            await _categoryRepository.AddAsync(category);
            await _categoryRepository.SaveChangesAsync();

            return new ApiResponseDto<CategoryResponseDto>
            {
                IsSuccess = true,
                StatusCode = 200,
                ErrorCode = "",
                Data = new CategoryResponseDto
                {
                    CategoryId = category.CategoryId,
                    CategoryName = category.CategoryName,
                    ParentCategoryId = category.ParentCategoryId
                },
                Message = "Category created successfully."
            };
        }

        public async Task<ApiResponseDto<CategoryResponseDto>> UpdateCategoryAsync(CreateCategoryRequestDto dto, int categoryId)
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId);

            if (category == null)
            {
                return new ApiResponseDto<CategoryResponseDto>
                {
                    IsSuccess = false,
                    StatusCode = 404,
                    ErrorCode = "CATEGORY_NOT_FOUND",
                    Message = "Category not found.",
                    Data = null
                };
            }

            if (dto.ParentCategoryId.HasValue && dto.ParentCategoryId.Value == categoryId)
            {
                return new ApiResponseDto<CategoryResponseDto>
                {
                    IsSuccess = false,
                    StatusCode = 400,
                    ErrorCode = "INVALID_PARENT_CATEGORY",
                    Message = "A category cannot be its own parent.",
                    Data = null
                };
            }

            var allCategories = await _categoryRepository.GetAllAsync();
            if (allCategories.Any(c => c.CategoryName.ToLower() == dto.CategoryName.ToLower() && c.CategoryId != categoryId))
            {
                return new ApiResponseDto<CategoryResponseDto>
                {
                    IsSuccess = false,
                    StatusCode = 409,
                    ErrorCode = "CATEGORY_NAME_EXISTS",
                    Message = "Another category with this name already exists.",
                    Data = null
                };
            }

            if (dto.ParentCategoryId.HasValue)
            {
                var parent = await _categoryRepository.GetByIdAsync(dto.ParentCategoryId.Value);
                if (parent == null)
                {
                    return new ApiResponseDto<CategoryResponseDto>
                    {
                        IsSuccess = false,
                        StatusCode = 400,
                        ErrorCode = "PARENT_CATEGORY_NOT_FOUND",
                        Message = "The specified parent category does not exist.",
                        Data = null
                    };
                }
            }

            category.CategoryName = dto.CategoryName;
            category.ParentCategoryId = dto.ParentCategoryId;

            _categoryRepository.Update(category);
            await _categoryRepository.SaveChangesAsync();

            return new ApiResponseDto<CategoryResponseDto>
            {
                IsSuccess = true,
                StatusCode = 200,
                ErrorCode = "",
                Data = new CategoryResponseDto
                {
                    CategoryId = category.CategoryId,
                    CategoryName = category.CategoryName,
                    ParentCategoryId = category.ParentCategoryId
                },
                Message = "Category updated successfully."
            };
        }

        public async Task<ApiResponseDto<string>> DeleteCategoryAsync(int categoryId)
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId);

            if (category == null)
            {
                return new ApiResponseDto<string>
                {
                    IsSuccess = false,
                    StatusCode = 404,
                    ErrorCode = "CATEGORY_NOT_FOUND",
                    Message = "Category not found.",
                    Data = null
                };
            }

            bool hasSubCategories = category.SubCategories != null && category.SubCategories.Any();
            bool hasProducts = category.Products != null && category.Products.Any();

            if (hasSubCategories || hasProducts)
            {
                return new ApiResponseDto<string>
                {
                    IsSuccess = false,
                    StatusCode = 400,
                    ErrorCode = "CATEGORY_IN_USE",
                    Message = "Cannot delete category because it contains products or subcategories.",
                    Data = null
                };
            }

            _categoryRepository.Delete(category);
            await _categoryRepository.SaveChangesAsync();

            return new ApiResponseDto<string>
            {
                IsSuccess = true,
                StatusCode = 200,
                ErrorCode = "",
                Data = null,
                Message = "Category deleted successfully."
            };
        }

        public async Task<ApiResponseDto<CategoryResponseDto>> GetCategoryByIdAsync(int categoryId)
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId);

            if (category == null)
            {
                return new ApiResponseDto<CategoryResponseDto>
                {
                    IsSuccess = false,
                    StatusCode = 404,
                    ErrorCode = "CATEGORY_NOT_FOUND",
                    Message = "Category not found.",
                    Data = null
                };
            }

            return new ApiResponseDto<CategoryResponseDto>
            {
                IsSuccess = true,
                StatusCode = 200,
                ErrorCode = "",
                Data = new CategoryResponseDto
                {
                    CategoryId = category.CategoryId,
                    CategoryName = category.CategoryName,
                    ParentCategoryId = category.ParentCategoryId,
                    SubCategories = category.SubCategories?.Select(s => new CategoryResponseDto
                    {
                        CategoryId = s.CategoryId,
                        CategoryName = s.CategoryName,
                        ParentCategoryId = s.ParentCategoryId
                    }).ToList()
                },
                Message = "Category retrieved successfully."
            };
        }

        public async Task<ApiResponseDto<PaginatedResponseDto<CategoryResponseDto>>> GetAllCategoriesAsync(PaginationRequestDto paginationRequestDto)
        {
            var (items, totalCount) = await _categoryRepository.GetAllPagedAsync(paginationRequestDto.PageNumber, paginationRequestDto.PageSize);


            var mappedCategories = items.Select(c => new CategoryResponseDto
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName,
                ParentCategoryId = c.ParentCategoryId
            }).ToList();

            int totalPages = (int)Math.Ceiling(totalCount / (double)paginationRequestDto.PageSize);

            return new ApiResponseDto<PaginatedResponseDto<CategoryResponseDto>>
            {
                IsSuccess = true,
                StatusCode = 200,
                ErrorCode = "",
                Data = new PaginatedResponseDto<CategoryResponseDto>
                {
                    Data = mappedCategories,
                    CurrentPage = paginationRequestDto.PageNumber,
                    TotalPages = totalPages, 
                    TotalCount = totalCount,
                    PageSize = paginationRequestDto.PageSize
                },
                Message = "Categories retrieved successfully."
            };
        }
    }
}

