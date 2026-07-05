using AutoMapper;
using Microsoft.Extensions.Logging;
using WebApplication1.DTOS.Request_DTOs;
using WebApplication1.DTOS.Response_DTOs;
using WebApplication1.DTOS.Shared.RequestDto;
using WebApplication1.DTOS.Shared.Response_DTOs;
using WebApplication1.Entitys;
using WebApplication1.Exceptions;
using WebApplication1.Repository.SpecificRepository.CategoryRepository.Interface;
using WebApplication1.Repository.SpecificRepository.ProductRepository;
using WebApplication1.Repository.SpecificRepository.SellerRepository;

namespace WebApplication1.Services.ProductService
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ISellerRepository _sellerRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductService> _logger;

        public ProductService(
            IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            ISellerRepository sellerRepository,
            IMapper mapper,
            ILogger<ProductService> logger)
        {
            _mapper = mapper;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _sellerRepository = sellerRepository;
            _logger = logger;
        }

        public async Task<ApiResponseDto<ProductResponseDto>> CreateProductAsync(CreateProductRequestDto createProductRequestDto, int sellerId)
        {
            var category = await _categoryRepository.GetByIdAsync(createProductRequestDto.CategoryId);
            if (category == null)
            {
                _logger.LogWarning("Seller {SellerId} attempted to create a " +
                    "product in non-existent CategoryId {CategoryId}.", sellerId, createProductRequestDto.CategoryId);
                throw new NotFoundException("The specified category does not exist.");
            }

            var seller = await _sellerRepository.GetByIdAsync(sellerId);
            if (seller == null)
            {
                _logger.LogWarning("Attempted to create a product for non-existent SellerId {SellerId}.", sellerId);
                throw new NotFoundException("The seller profile was not found.");
            }

            var product = _mapper.Map<Product>(createProductRequestDto);
            product.SellerId = sellerId;
            product.CreatedAt = DateTime.UtcNow;

            await _productRepository.AddAsync(product);
            await _productRepository.SaveChangesAsync();

            _logger.LogInformation("Seller {SellerId} successfully created a new product {ProductId}.", sellerId, product.ProductId);

            return new ApiResponseDto<ProductResponseDto>
            {
                Message = "Product created successfully.",
                Data = _mapper.Map<ProductResponseDto>(product)
            };
        }

        public async Task<ApiResponseDto<ProductResponseDto>> UpdateProductAsync(UpdateProductRequestDto updateProductRequestDto, int sellerId, int productId)
        {
            var product = await _productRepository.GetProductWithVariantsAsync(productId);

            if (product == null || product.IsDeleted)
            {
                _logger.LogWarning("Seller {SellerId} attempted to update non-existent or deleted ProductId {ProductId}.", sellerId, productId);
                throw new NotFoundException("The product does not exist.");
            }

            if (product.SellerId != sellerId)
            {
                _logger.LogWarning("Security Warning: Seller {SellerId} attempted" +
                    " to update ProductId {ProductId} belonging to another seller.", sellerId, productId);
                throw new UnauthorizedException("You do not have permission to update this product.");
            }

            var category = await _categoryRepository.GetByIdAsync(updateProductRequestDto.CategoryId);
            if (category == null)
            {
                _logger.LogWarning("Seller {SellerId} attempted to update ProductId " +
                    "{ProductId} with non-existent CategoryId {CategoryId}.", sellerId, productId, updateProductRequestDto.CategoryId);
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

            _productRepository.Update(product);
            await _productRepository.SaveChangesAsync();

            _logger.LogInformation("Seller {SellerId} successfully updated ProductId {ProductId} and its variants.", sellerId, productId);

            return new ApiResponseDto<ProductResponseDto>
            {
                Message = "Product updated successfully.",
                Data = _mapper.Map<ProductResponseDto>(product)
            };
        }

        public async Task<ApiResponseDto<string>> DeleteProductAsync(int sellerId, int productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null || product.IsDeleted)
            {
                _logger.LogWarning("Seller {SellerId} attempted to delete non-existent or already deleted ProductId {ProductId}.", sellerId, productId);
                throw new NotFoundException("The product does not exist.");
            }

            if (product.SellerId != sellerId)
            {
                _logger.LogWarning("Security Warning: Seller {SellerId}" +
                    " attempted to delete ProductId {ProductId} belonging to another seller.", sellerId, productId);
                throw new UnauthorizedException("You do not have permission to delete this product.");
            }

            product.IsDeleted = true;
            _productRepository.Update(product);
            await _productRepository.SaveChangesAsync();

            _logger.LogInformation("Seller {SellerId} successfully soft-deleted ProductId {ProductId}.", sellerId, productId);

            return new ApiResponseDto<string>
            {
                Message = "Product deleted successfully.",
                Data = null
            };
        }

        public async Task<ApiResponseDto<ProductResponseDto>> GetProductByIdAsync(int productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
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

        public async Task<ApiResponseDto<IEnumerable<ProductResponseDto>>> GetOutOfStockProductsAsync(int sellerId)
        {
            var allOutOfStock = await _productRepository.GetOutOfStockProductsAsync();

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

        public async Task<ApiResponseDto<PaginatedResponseDto<ProductResponseDto>>> GetAllProductsAsync(int? categoryId, PaginationRequestDto paginationRequestDto)
        {
            var (products, totalCount) = await _productRepository.GetProductsPagedAsync(
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
    }
}