using AutoMapper;
using Microsoft.Extensions.Logging;
using WebApplication1.DTOS.Request_DTOs;
using WebApplication1.DTOS.Request_DTOs.Category;
using WebApplication1.DTOS.Response_DTOs;
using WebApplication1.DTOS.Shared.RequestDto;
using WebApplication1.DTOS.Shared.Response_DTOs;
using WebApplication1.Entitys;
using WebApplication1.Exceptions;
using WebApplication1.Repository.SpecificRepository.CategoryRepository.Interface;

namespace WebApplication1.Services.CategoryService
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(
            ICategoryRepository categoryRepository,
            IMapper mapper,
            ILogger<CategoryService> logger)
        {
            _mapper = mapper;
            _categoryRepository = categoryRepository;
            _logger = logger;
        }

        public async Task<ApiResponseDto<CategoryResponseDto>> CreateCategoryAsync(CreateCategoryRequestDto createCategoryRequestDto)
        {
            var allCategories = await _categoryRepository.GetAllAsync();

            if (allCategories.Any(c => c.CategoryName.ToLower() == createCategoryRequestDto.CategoryName.ToLower()))
            {
                _logger.LogWarning("Attempted to create a category with an already existing name: {CategoryName}.", createCategoryRequestDto.CategoryName);
                throw new ConflictException("A category with this name already exists.");
            }

            if (createCategoryRequestDto.ParentCategoryId.HasValue)
            {
                var parent = await _categoryRepository.GetByIdAsync(createCategoryRequestDto.ParentCategoryId.Value);
                if (parent == null)
                {
                    _logger.LogWarning("Attempted to create category {CategoryName} with non-existent ParentCategoryId " +
                        "{ParentCategoryId}.", createCategoryRequestDto.CategoryName, createCategoryRequestDto.ParentCategoryId.Value);
                    throw new NotFoundException("The specified parent category does not exist.");
                }
            }

            var category = _mapper.Map<Category>(createCategoryRequestDto);

            await _categoryRepository.AddAsync(category);
            await _categoryRepository.SaveChangesAsync();

            _logger.LogInformation("Successfully created new category {CategoryId} with name {CategoryName}.", category.CategoryId, category.CategoryName);

            return new ApiResponseDto<CategoryResponseDto>
            {
                Data = _mapper.Map<CategoryResponseDto>(category),
                Message = "Category created successfully."
            };
        }

        public async Task<ApiResponseDto<CategoryResponseDto>> UpdateCategoryAsync(UpdateCategoryRequestDto updateCategoryRequestDto, int categoryId)
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId);

            if (category == null)
            {
                _logger.LogWarning("Attempted to update non-existent CategoryId {CategoryId}.", categoryId);
                throw new NotFoundException("Category not found.");
            }

            if (updateCategoryRequestDto.ParentCategoryId.HasValue && updateCategoryRequestDto.ParentCategoryId.Value == categoryId)
            {
                _logger.LogWarning("Attempted to set CategoryId {CategoryId} as its own parent. This could cause infinite loops.", categoryId);
                throw new BadRequestException("A category cannot be its own parent.");
            }

            var allCategories = await _categoryRepository.GetAllAsync();
            if (allCategories.Any(c => c.CategoryName.ToLower() == updateCategoryRequestDto.CategoryName.ToLower() && c.CategoryId != categoryId))
            {
                _logger.LogWarning("Attempted to update CategoryId {CategoryId} to an already existing name: {CategoryName}.", categoryId, updateCategoryRequestDto.CategoryName);
                throw new ConflictException("Another category with this name already exists.");
            }

            if (updateCategoryRequestDto.ParentCategoryId.HasValue)
            {
                var parent = await _categoryRepository.GetByIdAsync(updateCategoryRequestDto.ParentCategoryId.Value);
                if (parent == null)
                {
                    _logger.LogWarning("Attempted to update CategoryId {CategoryId} with non-existent " +
                        "ParentCategoryId {ParentCategoryId}.", categoryId, updateCategoryRequestDto.ParentCategoryId.Value);
                    throw new NotFoundException("The specified parent category does not exist.");
                }
            }

            _mapper.Map(updateCategoryRequestDto, category);

            _categoryRepository.Update(category);
            await _categoryRepository.SaveChangesAsync();

            _logger.LogInformation("Successfully updated CategoryId {CategoryId}.", categoryId);

            return new ApiResponseDto<CategoryResponseDto>
            {
                Data = _mapper.Map<CategoryResponseDto>(category),
                Message = "Category updated successfully."
            };
        }

        public async Task<ApiResponseDto<string>> DeleteCategoryAsync(int categoryId)
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId);

            if (category == null)
            {
                _logger.LogWarning("Attempted to delete non-existent CategoryId {CategoryId}.", categoryId);
                throw new NotFoundException("Category not found.");
            }

            bool hasSubCategories = category.SubCategories != null && category.SubCategories.Any();
            bool hasProducts = category.Products != null && category.Products.Any();

            if (hasSubCategories || hasProducts)
            {
                _logger.LogWarning("Attempted to delete CategoryId {CategoryId} which is not empty." +
                    " HasSubCategories: {HasSubCategories}, HasProducts: {HasProducts}."
                    , categoryId, hasSubCategories, hasProducts);
                throw new BadRequestException("Cannot delete category because it contains products or subcategories.");
            }

            _categoryRepository.Delete(category);
            await _categoryRepository.SaveChangesAsync();

            _logger.LogInformation("Successfully deleted CategoryId {CategoryId}.", categoryId);

            return new ApiResponseDto<string>
            {
                Data = null,
                Message = "Category deleted successfully."
            };
        }

        public async Task<ApiResponseDto<CategoryResponseDto>> GetCategoryByIdAsync(int categoryId)
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId);

            if (category == null)
            {
                _logger.LogWarning("Attempted to retrieve non-existent CategoryId {CategoryId}.", categoryId);
                throw new NotFoundException("Category not found.");
            }

            return new ApiResponseDto<CategoryResponseDto>
            {
                Data = _mapper.Map<CategoryResponseDto>(category),
                Message = "Category retrieved successfully."
            };
        }

        public async Task<ApiResponseDto<PaginatedResponseDto<CategoryResponseDto>>> GetAllCategoriesAsync(PaginationRequestDto paginationRequestDto)
        {
            var (items, totalCount) = await _categoryRepository.GetAllPagedAsync(paginationRequestDto.PageNumber, paginationRequestDto.PageSize);

            var mappedCategories = _mapper.Map<List<CategoryResponseDto>>(items);

            int totalPages = (int)Math.Ceiling(totalCount / (double)paginationRequestDto.PageSize);

            return new ApiResponseDto<PaginatedResponseDto<CategoryResponseDto>>
            {
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