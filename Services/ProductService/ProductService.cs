using AutoMapper;
using WebApplication1.DTOS.Request_DTOs;
using WebApplication1.DTOS.Response_DTOs;
using WebApplication1.DTOS.Shared.RequestDto;
using WebApplication1.DTOS.Shared.Response_DTOs;
using WebApplication1.Entities;
using WebApplication1.Entitys;
using WebApplication1.Repository.SpecificRepository.CategoryRepository.Interface;
using WebApplication1.Repository.SpecificRepository.ProductRepository;
using WebApplication1.Repository.SpecificRepository.SellerRepository;
using WebApplication1.Services.Interface;

namespace WebApplication1.Services.ProductService
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ISellerRepository _sellerRepository;
        private readonly IMapper _mapper;
        public ProductService(
            IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            ISellerRepository sellerRepository,
            IMapper mapper
            )
        {
            _mapper = mapper;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _sellerRepository = sellerRepository;
        }

        public async Task<ApiResponseDto<ProductResponseDto>> CreateProductAsync(CreateProductRequestDto createProductRequestDto, int SellerId)
        {
            var category = await _categoryRepository.GetByIdAsync(createProductRequestDto.CategoryId);
            if (category == null)
            {
                return new ApiResponseDto<ProductResponseDto>
                {
                    IsSuccess = false,
                    StatusCode = 404,
                    ErrorCode = "CATEGORY_NOT_FOUND",
                    Data = null,
                    Message = "The specified category does not exist."
                };
            }

            var seller = await _sellerRepository.GetByIdAsync(SellerId);
            if (seller == null)
            {
                return new ApiResponseDto<ProductResponseDto>
                {
                    IsSuccess = false,
                    StatusCode = 404,
                    ErrorCode = "SELLER_NOT_FOUND",
                    Data = null,
                    Message = "The seller profile was not found."
                };
            }

            var product = _mapper.Map<Product>(createProductRequestDto);
            product.SellerId = SellerId;
            product.CreatedAt= DateTime.UtcNow;


            await _productRepository.AddAsync(product);
            await _productRepository.SaveChangesAsync();

            return new ApiResponseDto<ProductResponseDto>
            {
                IsSuccess = true,
                StatusCode = 200,
                ErrorCode = "",
                Message = "Product created successfully.",
                Data = _mapper.Map<ProductResponseDto>(product)
            };
        }

        public async Task<ApiResponseDto<ProductResponseDto>> UpdateProductAsync(UpdateProductRequestDto updateProductRequestDto, int SellerId, int ProductId)
        {

            var product = await _productRepository.GetProductWithVariantsAsync(ProductId);

            if (product == null || product.IsDeleted)
            {
                return new ApiResponseDto<ProductResponseDto>
                {
                    IsSuccess = false,
                    StatusCode = 404,
                    ErrorCode = "PRODUCT_NOT_FOUND",
                    Data = null,
                    Message = "The product does not exist."
                };
            }

            if (product.SellerId != SellerId)
            {
                return new ApiResponseDto<ProductResponseDto>
                {
                    IsSuccess = false,
                    StatusCode = 403,
                    ErrorCode = "UNAUTHORIZED_ACTION",
                    Data = null,
                    Message = "You do not have permission to update this product."
                };
            }

            var category = await _categoryRepository.GetByIdAsync(updateProductRequestDto.CategoryId);
            if (category == null)
            {
                return new ApiResponseDto<ProductResponseDto>
                {
                    IsSuccess = false,
                    StatusCode = 404,
                    ErrorCode = "CATEGORY_NOT_FOUND",
                    Data = null,
                    Message = "The specified category does not exist."
                };
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

            var seller = await _sellerRepository.GetByIdAsync(SellerId);

            return new ApiResponseDto<ProductResponseDto>
            {
                IsSuccess = true,
                StatusCode = 200,
                ErrorCode = "",
                Message = "Product updated successfully.",
                Data = _mapper.Map<ProductResponseDto>(product)
            };
        }

        public async Task<ApiResponseDto<string>> DeleteProductAsync(int SellerId, int ProductId)
        {
            var product = await _productRepository.GetByIdAsync(ProductId);
            if (product == null || product.IsDeleted)
            {
                return new ApiResponseDto<string>
                {
                    IsSuccess = false,
                    StatusCode = 404,
                    ErrorCode = "PRODUCT_NOT_FOUND",
                    Data = null,
                    Message = "The product does not exist."
                };
            }

            if (product.SellerId != SellerId)
            {
                return new ApiResponseDto<string>
                {
                    IsSuccess = false,
                    StatusCode = 403,
                    ErrorCode = "UNAUTHORIZED_ACTION",
                    Data = null,
                    Message = "You do not have permission to delete this product."
                };
            }

            product.IsDeleted = true;
            _productRepository.Update(product);
            await _productRepository.SaveChangesAsync();

            return new ApiResponseDto<string>
            {
                IsSuccess = true,
                StatusCode = 200,
                ErrorCode = "",
                Message = "Product deleted successfully.",
                Data = null
            };
        }

        public async Task<ApiResponseDto<ProductResponseDto>> GetProductByIdAsync(int ProductId)
        {
            var product = await _productRepository.GetByIdAsync(ProductId);
            if (product == null || product.IsDeleted)
            {
                return new ApiResponseDto<ProductResponseDto>
                {
                    IsSuccess = false,
                    StatusCode = 404,
                    ErrorCode = "PRODUCT_NOT_FOUND",
                    Data = null,
                    Message = "The product does not exist."
                };
            }

            var category = await _categoryRepository.GetByIdAsync(product.CategoryId);
            var seller = await _sellerRepository.GetByIdAsync(product.SellerId);

            return new ApiResponseDto<ProductResponseDto>
            {
                IsSuccess = true,
                StatusCode = 200,
                ErrorCode = "",
                Message = "Product retrieved successfully.",
                Data = _mapper.Map<ProductResponseDto>(product)
            };
        }
        public async Task<ApiResponseDto<IEnumerable<ProductResponseDto>>> GetOutOfStockProductsAsync(int SellerId)
        {
            var allOutOfStock = await _productRepository.GetOutOfStockProductsAsync();

            var sellerOutOfStockProducts = allOutOfStock
                .Where(p => p.SellerId == SellerId && !p.IsDeleted)
                .ToList();

            var seller = await _sellerRepository.GetByIdAsync(SellerId);
            var mappedData = new List<ProductResponseDto>();    

            foreach (var product in sellerOutOfStockProducts)
            {
                var category = await _categoryRepository.GetByIdAsync(product.CategoryId);

                mappedData.Add(_mapper.Map<ProductResponseDto>(product));
            }

            return new ApiResponseDto<IEnumerable<ProductResponseDto>>
            {
                IsSuccess = true,
                StatusCode = 200,
                ErrorCode = "",
                Message = "Out of stock products retrieved successfully.",
                Data = mappedData
            };
        }

        public async Task<ApiResponseDto<PaginatedResponseDto<ProductResponseDto>>> GetAllProductsAsync(int? CategoryId, PaginationRequestDto paginationRequestDto)
        {
            IEnumerable<Product> sourceData;

            if (CategoryId.HasValue)
            {
                sourceData = await _productRepository.FindAsync(p => p.CategoryId == CategoryId.Value && !p.IsDeleted);
            }
            else
            {
                var allProducts = await _productRepository.GetAllAsync();
                sourceData = allProducts.Where(p => !p.IsDeleted);
            }

            var totalCount = sourceData.Count();
            int totalPages = (int)Math.Ceiling(totalCount / (double)paginationRequestDto.PageSize);

            var pagedProducts = sourceData
                .Skip((paginationRequestDto.PageNumber - 1) * paginationRequestDto.PageSize)
                .Take(paginationRequestDto.PageSize)
                .ToList();

            var mappedData = new List<ProductResponseDto>();
            foreach (var p in pagedProducts)
            { 
                mappedData.Add(_mapper.Map<ProductResponseDto>(p));
            }

            return new ApiResponseDto<PaginatedResponseDto<ProductResponseDto>>
            {
                IsSuccess = true,
                StatusCode = 200,
                ErrorCode = "",
                Message = "Products retrieved successfully.",
                Data = new PaginatedResponseDto<ProductResponseDto>
                {
                    CurrentPage = paginationRequestDto.PageNumber,
                    PageSize = paginationRequestDto.PageSize,
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    Data = mappedData
                }
            };
        }
    }
}