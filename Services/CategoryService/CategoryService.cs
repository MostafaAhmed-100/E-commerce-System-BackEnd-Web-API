using AutoMapper;
using WebApplication1.DTOS.Request_DTOs;
using WebApplication1.DTOS.Request_DTOs.Category;
using WebApplication1.DTOS.Response_DTOs;
using WebApplication1.DTOS.Shared.RequestDto;
using WebApplication1.DTOS.Shared.Response_DTOs;
using WebApplication1.Entitys;
using WebApplication1.Exceptions;
using WebApplication1.Repository.UnitOfWork;

namespace WebApplication1.Services.CategoryService
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<CategoryService> logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ApiResponseDto<CategoryResponseDto>> CreateCategoryAsync(CreateCategoryRequestDto createCategoryRequestDto)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var existingCategories = await _unitOfWork.CategoryRepository.FindAsync(c => c.CategoryName.ToLower() == createCategoryRequestDto.CategoryName.ToLower());

                if (existingCategories.Any())
                {
                    _logger.LogWarning("Attempted to create a category with an already existing name: {CategoryName}.", createCategoryRequestDto.CategoryName);
                    throw new ConflictException("A category with this name already exists.");
                }

                if (createCategoryRequestDto.ParentCategoryId.HasValue)
                {
                    var parent = await _unitOfWork.CategoryRepository.GetByIdAsync(createCategoryRequestDto.ParentCategoryId.Value);
                    if (parent == null)
                    {
                        _logger.LogWarning("Attempted to create category {CategoryName} with non-existent ParentCategoryId {ParentCategoryId}.", createCategoryRequestDto.CategoryName, createCategoryRequestDto.ParentCategoryId.Value);
                        throw new NotFoundException("The specified parent category does not exist.");
                    }
                }

                var category = _mapper.Map<Category>(createCategoryRequestDto);

                await _unitOfWork.CategoryRepository.AddAsync(category);
                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Successfully created new category {CategoryId} with name {CategoryName}.", category.CategoryId, category.CategoryName);

                return new ApiResponseDto<CategoryResponseDto>
                {
                    Data = _mapper.Map<CategoryResponseDto>(category),
                    Message = "Category created successfully."
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error occurred while creating category {CategoryName}", createCategoryRequestDto.CategoryName);
                throw;
            }
        }

        public async Task<ApiResponseDto<CategoryResponseDto>> UpdateCategoryAsync(UpdateCategoryRequestDto updateCategoryRequestDto, int categoryId)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var category = await _unitOfWork.CategoryRepository.GetByIdAsync(categoryId);

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

                var existingCategories = await _unitOfWork.CategoryRepository.FindAsync(c => c.CategoryName.ToLower() == updateCategoryRequestDto.CategoryName.ToLower() && c.CategoryId != categoryId);

                if (existingCategories.Any())
                {
                    _logger.LogWarning("Attempted to update CategoryId {CategoryId} to an already existing name: {CategoryName}.", categoryId, updateCategoryRequestDto.CategoryName);
                    throw new ConflictException("Another category with this name already exists.");
                }

                if (updateCategoryRequestDto.ParentCategoryId.HasValue)
                {
                    var parent = await _unitOfWork.CategoryRepository.GetByIdAsync(updateCategoryRequestDto.ParentCategoryId.Value);
                    if (parent == null)
                    {
                        _logger.LogWarning("Attempted to update CategoryId {CategoryId} with non-existent ParentCategoryId {ParentCategoryId}.", categoryId, updateCategoryRequestDto.ParentCategoryId.Value);
                        throw new NotFoundException("The specified parent category does not exist.");
                    }
                }

                _mapper.Map(updateCategoryRequestDto, category);

                _unitOfWork.CategoryRepository.Update(category);
                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Successfully updated CategoryId {CategoryId}.", categoryId);

                return new ApiResponseDto<CategoryResponseDto>
                {
                    Data = _mapper.Map<CategoryResponseDto>(category),
                    Message = "Category updated successfully."
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error occurred while updating CategoryId {CategoryId}", categoryId);
                throw;
            }
        }

        public async Task<ApiResponseDto<string>> DeleteCategoryAsync(int categoryId)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var category = await _unitOfWork.CategoryRepository.GetByIdAsync(categoryId);

                if (category == null)
                {
                    _logger.LogWarning("Attempted to delete non-existent CategoryId {CategoryId}.", categoryId);
                    throw new NotFoundException("Category not found.");
                }

                bool hasSubCategories = category.SubCategories != null && category.SubCategories.Any();
                bool hasProducts = category.Products != null && category.Products.Any();

                if (hasSubCategories || hasProducts)
                {
                    _logger.LogWarning("Attempted to delete CategoryId {CategoryId} which is not empty. HasSubCategories: {HasSubCategories}, HasProducts: {HasProducts}.", categoryId, hasSubCategories, hasProducts);
                    throw new BadRequestException("Cannot delete category because it contains products or subcategories.");
                }

                _unitOfWork.CategoryRepository.Delete(category);
                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Successfully deleted CategoryId {CategoryId}.", categoryId);

                return new ApiResponseDto<string>
                {
                    Data = null,
                    Message = "Category deleted successfully."
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error occurred while deleting CategoryId {CategoryId}", categoryId);
                throw;
            }
        }

        public async Task<ApiResponseDto<CategoryResponseDto>> GetCategoryByIdAsync(int categoryId)
        {
            try
            {
                var category = await _unitOfWork.CategoryRepository.GetByIdAsync(categoryId);

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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving CategoryId {CategoryId}", categoryId);
                throw;
            }
        }

        public async Task<ApiResponseDto<PaginatedResponseDto<CategoryResponseDto>>> GetAllCategoriesAsync(PaginationRequestDto paginationRequestDto)
        {
            try
            {
                var (items, totalCount) = await _unitOfWork.CategoryRepository.GetAllPagedAsync(paginationRequestDto.PageNumber, paginationRequestDto.PageSize);

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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all categories");
                throw;
            }
        }
    }
}