using AutoMapper;
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

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _mapper = mapper;
            _categoryRepository = categoryRepository;
        }

        public async Task<ApiResponseDto<CategoryResponseDto>> CreateCategoryAsync(CreateCategoryRequestDto createCategoryRequestDto)
        {
            var allCategories = await _categoryRepository.GetAllAsync();

            if (allCategories.Any(c => c.CategoryName.ToLower() == createCategoryRequestDto.CategoryName.ToLower()))
                throw new ConflictException("A category with this name already exists.");

            if (createCategoryRequestDto.ParentCategoryId.HasValue)
            {
                var parent = await _categoryRepository.GetByIdAsync(createCategoryRequestDto.ParentCategoryId.Value);
                if (parent == null)
                    throw new NotFoundException("The specified parent category does not exist.");
            }

            var category = _mapper.Map<Category>(createCategoryRequestDto);

            await _categoryRepository.AddAsync(category);
            await _categoryRepository.SaveChangesAsync();

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
                throw new NotFoundException("Category not found.");

            if (updateCategoryRequestDto.ParentCategoryId.HasValue && updateCategoryRequestDto.ParentCategoryId.Value == categoryId)
                throw new BadRequestException("A category cannot be its own parent.");

            var allCategories = await _categoryRepository.GetAllAsync();
            if (allCategories.Any(c => c.CategoryName.ToLower() == updateCategoryRequestDto.CategoryName.ToLower() && c.CategoryId != categoryId))
                throw new ConflictException("Another category with this name already exists.");

            if (updateCategoryRequestDto.ParentCategoryId.HasValue)
            {
                var parent = await _categoryRepository.GetByIdAsync(updateCategoryRequestDto.ParentCategoryId.Value);
                if (parent == null)
                    throw new NotFoundException("The specified parent category does not exist.");
            }

            _mapper.Map(updateCategoryRequestDto, category);

            _categoryRepository.Update(category);
            await _categoryRepository.SaveChangesAsync();

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
                throw new NotFoundException("Category not found.");

            bool hasSubCategories = category.SubCategories != null && category.SubCategories.Any();
            bool hasProducts = category.Products != null && category.Products.Any();

            if (hasSubCategories || hasProducts)
                throw new BadRequestException("Cannot delete category because it contains products or subcategories.");

            _categoryRepository.Delete(category);
            await _categoryRepository.SaveChangesAsync();

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
                throw new NotFoundException("Category not found.");

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