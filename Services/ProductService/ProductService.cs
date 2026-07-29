using AutoMapper;
using WebApplication1.DTOS.Request_DTOs;
using WebApplication1.DTOS.Response_DTOs;
using WebApplication1.DTOS.Shared.RequestDto;
using WebApplication1.DTOS.Shared.Response_DTOs;
using WebApplication1.Entitys;
using WebApplication1.Exceptions;
using WebApplication1.Repository.UnitOfWork;

namespace WebApplication1.Services.ProductService
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductService> _logger;

        public ProductService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<ProductService> logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ApiResponseDto<ProductResponseDto>> CreateProductAsync(CreateProductRequestDto createProductRequestDto, int sellerId)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var category = await _unitOfWork.CategoryRepository.GetByIdAsync(createProductRequestDto.CategoryId);
                if (category == null)
                {
                    _logger.LogWarning("Seller {SellerId} attempted to create a product in non-existent CategoryId {CategoryId}.", sellerId, createProductRequestDto.CategoryId);
                    throw new NotFoundException("The specified category does not exist.");
                }

                var seller = await _unitOfWork.SellerRepository.GetByIdAsync(sellerId);
                if (seller == null)
                {
                    _logger.LogWarning("Attempted to create a product for non-existent SellerId {SellerId}.", sellerId);
                    throw new NotFoundException("The seller profile was not found.");
                }

                var product = _mapper.Map<Product>(createProductRequestDto);
                product.SellerId = sellerId;
                product.CreatedAt = DateTime.UtcNow;

                await _unitOfWork.ProductRepository.AddAsync(product);
                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Seller {SellerId} successfully created a new product {ProductId}.", sellerId, product.ProductId);

                return new ApiResponseDto<ProductResponseDto>
                {
                    Message = "Product created successfully.",
                    Data = _mapper.Map<ProductResponseDto>(product)
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error occurred while creating product for Seller {SellerId}", sellerId);
                throw;
            }
        }

        public async Task<ApiResponseDto<ProductResponseDto>> UpdateProductAsync(UpdateProductRequestDto updateProductRequestDto, int sellerId, int productId)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var product = await _unitOfWork.ProductRepository.GetProductWithVariantsAsync(productId);

                if (product == null || product.IsDeleted)
                {
                    _logger.LogWarning("Seller {SellerId} attempted to update non-existent or deleted ProductId {ProductId}.", sellerId, productId);
                    throw new NotFoundException("The product does not exist.");
                }

                if (product.SellerId != sellerId)
                {
                    _logger.LogWarning("Security Warning: Seller {SellerId} attempted to update ProductId {ProductId} belonging to another seller.", sellerId, productId);
                    throw new UnauthorizedException("You do not have permission to update this product.");
                }

                var category = await _unitOfWork.CategoryRepository.GetByIdAsync(updateProductRequestDto.CategoryId);
                if (category == null)
                {
                    _logger.LogWarning("Seller {SellerId} attempted to update ProductId {ProductId} with non-existent CategoryId {CategoryId}.", sellerId, productId, updateProductRequestDto.CategoryId);
                    throw new NotFoundException("The specified category does not exist.");
                }

                product.ProductName = updateProductRequestDto.ProductName;
                product.ProductDescription = updateProductRequestDto.ProductDescription;
                product.CategoryId = updateProductRequestDto.CategoryId;

                product.ProductVariants ??= new List<ProductVariant>();

                var incomingVariantIds = updateProductRequestDto.Variants
                    .Where(v => v.VariantId.HasValue)
                    .Select(v => v.VariantId.Value)
                    .ToList();

                var variantsToRemove = product.ProductVariants
                    .Where(v => !incomingVariantIds.Contains(v.ProductVariantId))
                    .ToList();

                foreach (var variant in variantsToRemove)
                {
                    product.ProductVariants.Remove(variant);
                }

                foreach (var incomingVariant in updateProductRequestDto.Variants)
                {
                    if (incomingVariant.VariantId.HasValue)
                    {
                        var existingVariant = product.ProductVariants
                            .FirstOrDefault(v => v.ProductVariantId == incomingVariant.VariantId.Value);

                        if (existingVariant != null)
                        {
                            existingVariant.SKU = incomingVariant.SKU;
                            existingVariant.Price = incomingVariant.Price;
                            existingVariant.QuantityInStock = incomingVariant.QuantityInStock;
                            existingVariant.Color = incomingVariant.Color ?? "";
                            existingVariant.Size = incomingVariant.Size ?? "";
                        }
                    }
                    else
                    {
                        product.ProductVariants.Add(new ProductVariant
                        {
                            SKU = incomingVariant.SKU,
                            Price = incomingVariant.Price,
                            QuantityInStock = incomingVariant.QuantityInStock,
                            Color = incomingVariant.Color ?? "",
                            Size = incomingVariant.Size ?? "",
                            Discount = 0,
                            ReservedQuantity = 0
                        });
                    }
                }

                _unitOfWork.ProductRepository.Update(product);
                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Seller {SellerId} successfully updated ProductId {ProductId} and its variants.", sellerId, productId);

                return new ApiResponseDto<ProductResponseDto>
                {
                    Message = "Product updated successfully.",
                    Data = _mapper.Map<ProductResponseDto>(product)
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error occurred while updating product {ProductId} for Seller {SellerId}", productId, sellerId);
                throw;
            }
        }

        public async Task<ApiResponseDto<string>> DeleteProductAsync(int sellerId, int productId)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var product = await _unitOfWork.ProductRepository.GetByIdAsync(productId);
                if (product == null || product.IsDeleted)
                {
                    _logger.LogWarning("Seller {SellerId} attempted to delete non-existent or already deleted ProductId {ProductId}.", sellerId, productId);
                    throw new NotFoundException("The product does not exist.");
                }

                if (product.SellerId != sellerId)
                {
                    _logger.LogWarning("Security Warning: Seller {SellerId} attempted to delete ProductId {ProductId} belonging to another seller.", sellerId, productId);
                    throw new UnauthorizedException("You do not have permission to delete this product.");
                }

                product.IsDeleted = true;
                _unitOfWork.ProductRepository.Update(product);
                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Seller {SellerId} successfully soft-deleted ProductId {ProductId}.", sellerId, productId);

                return new ApiResponseDto<string>
                {
                    Message = "Product deleted successfully.",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error occurred while deleting product {ProductId} for Seller {SellerId}", productId, sellerId);
                throw;
            }
        }

        public async Task<ApiResponseDto<ProductResponseDto>> GetProductByIdAsync(int productId)
        {
            try
            {
                var product = await _unitOfWork.ProductRepository.GetByIdAsync(productId);
                if (product == null || product.IsDeleted)
                {
                    _logger.LogWarning("Attempted to retrieve non-existent or deleted ProductId {ProductId}.", productId);
                    throw new NotFoundException("The product does not exist.");
                }

                return new ApiResponseDto<ProductResponseDto>
                {
                    Message = "Product retrieved successfully.",
                    Data = _mapper.Map<ProductResponseDto>(product)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving product {ProductId}", productId);
                throw;
            }
        }

        public async Task<ApiResponseDto<IEnumerable<ProductResponseDto>>> GetOutOfStockProductsAsync(int sellerId)
        {
            try
            {
                var allOutOfStock = await _unitOfWork.ProductRepository.GetOutOfStockProductsAsync();

                var sellerOutOfStockProducts = allOutOfStock
                    .Where(p => p.SellerId == sellerId && !p.IsDeleted)
                    .ToList();

                var mappedData = _mapper.Map<IEnumerable<ProductResponseDto>>(sellerOutOfStockProducts);

                return new ApiResponseDto<IEnumerable<ProductResponseDto>>
                {
                    Message = "Out of stock products retrieved successfully.",
                    Data = mappedData
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving out of stock products for seller {SellerId}", sellerId);
                throw;
            }
        }

        public async Task<ApiResponseDto<PaginatedResponseDto<ProductResponseDto>>> GetAllProductsAsync(int? categoryId, PaginationRequestDto paginationRequestDto)
        {
            try
            {
                var (products, totalCount) = await _unitOfWork.ProductRepository.GetProductsPagedAsync(
                    categoryId,
                    paginationRequestDto.PageNumber,
                    paginationRequestDto.PageSize
                );

                int totalPages = (int)Math.Ceiling(totalCount / (double)paginationRequestDto.PageSize);

                var mappedData = _mapper.Map<List<ProductResponseDto>>(products);

                return new ApiResponseDto<PaginatedResponseDto<ProductResponseDto>>
                {
                    Message = "Products retrieved successfully.",
                    Data = new PaginatedResponseDto<ProductResponseDto>
                    {
                        CurrentPage = paginationRequestDto.PageNumber,
                        PageSize = paginationRequestDto.PageSize,
                        TotalCount = totalCount,
                        TotalPages = totalPages,
                        Data = mappedData!
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving paged products");
                throw;
            }
        }
    }
}