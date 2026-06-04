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

        public ProductService(
            IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            ISellerRepository sellerRepository)
        {
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

            var product = new Product
            {
                CategoryId = createProductRequestDto.CategoryId,
                SellerId = SellerId,
                ProductName = createProductRequestDto.ProductName,
                ProductDescription = createProductRequestDto.ProductDescription,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false,
                ProductVariants = createProductRequestDto.Variants.Select(v => new ProductVariant
                {
                    SKU = v.SKU,
                    Price = v.Price,
                    QuantityInStock = v.QuantityInStock,
                    Color = v.Color ?? "",
                    Size = v.Size ?? "",
                    Discount = 0,
                    ReservedQuantity = 0
                }).ToList()
            };

            await _productRepository.AddAsync(product);
            await _productRepository.SaveChangesAsync();

            return new ApiResponseDto<ProductResponseDto>
            {
                IsSuccess = true,
                StatusCode = 200,
                ErrorCode = "",
                Message = "Product created successfully.",
                Data = new ProductResponseDto
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    ProductDescription = product.ProductDescription,
                    CategoryId = product.CategoryId,
                    CategoryName = category.CategoryName,
                    SellerId = seller.SellerId,
                    SellerStoreName = seller.StoreName,
                    Variants = product.ProductVariants.Select(v => new ProductVariantResponseDto
                    {
                        VariantId = v.ProductVariantId,
                        SKU = v.SKU,
                        Price = v.Price,
                        IsAvailable = v.QuantityInStock > 0,
                        Color = v.Color,
                        Size = v.Size
                    }).ToList()
                }
            };
        }

        public async Task<ApiResponseDto<ProductResponseDto>> UpdateProductAsync(CreateProductRequestDto createProductRequestDto, int SellerId, int ProductId)
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

            product.ProductName = createProductRequestDto.ProductName;
            product.ProductDescription = createProductRequestDto.ProductDescription;
            product.CategoryId = createProductRequestDto.CategoryId;

            product.ProductVariants = createProductRequestDto.Variants.Select(v => new ProductVariant
            {
                SKU = v.SKU,
                Price = v.Price,
                QuantityInStock = v.QuantityInStock,
                Color = v.Color ?? "",
                Size = v.Size ?? "",
                Discount = 0,
                ReservedQuantity = 0
            }).ToList();

            _productRepository.Update(product);
            await _productRepository.SaveChangesAsync();

            var seller = await _sellerRepository.GetByIdAsync(SellerId);

            return new ApiResponseDto<ProductResponseDto>
            {
                IsSuccess = true,
                StatusCode = 200,
                ErrorCode = "",
                Message = "Product updated successfully.",
                Data = new ProductResponseDto
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    ProductDescription = product.ProductDescription,
                    CategoryId = product.CategoryId,
                    CategoryName = category.CategoryName,
                    SellerId = seller!.SellerId,
                    SellerStoreName = seller.StoreName,
                    Variants = product.ProductVariants.Select(v => new ProductVariantResponseDto
                    {
                        VariantId = v.ProductVariantId,
                        SKU = v.SKU,
                        Price = v.Price,
                        IsAvailable = v.QuantityInStock > 0,
                        Color = v.Color,
                        Size = v.Size
                    }).ToList()
                }
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
                Data = new ProductResponseDto
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    ProductDescription = product.ProductDescription,
                    CategoryId = product.CategoryId,
                    CategoryName = category?.CategoryName ?? "Unknown",
                    SellerId = product.SellerId,
                    SellerStoreName = seller?.StoreName ?? "Unknown",
                    Variants = product.ProductVariants?.Select(v => new ProductVariantResponseDto
                    {
                        VariantId = v.ProductVariantId,
                        SKU = v.SKU,
                        Price = v.Price,
                        IsAvailable = v.QuantityInStock > 0,
                        Color = v.Color,
                        Size = v.Size
                    }).ToList() ?? new List<ProductVariantResponseDto>()
                }
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
                var cat = await _categoryRepository.GetByIdAsync(p.CategoryId);
                var sel = await _sellerRepository.GetByIdAsync(p.SellerId);

                mappedData.Add(new ProductResponseDto
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    ProductDescription = p.ProductDescription,
                    CategoryId = p.CategoryId,
                    CategoryName = cat?.CategoryName ?? "Unknown",
                    SellerId = p.SellerId,
                    SellerStoreName = sel?.StoreName ?? "Unknown",
                    Variants = p.ProductVariants?.Select(v => new ProductVariantResponseDto
                    {
                        VariantId = v.ProductVariantId,
                        SKU = v.SKU,
                        Price = v.Price,
                        IsAvailable = v.QuantityInStock > 0,
                        Color = v.Color,
                        Size = v.Size
                    }).ToList() ?? new List<ProductVariantResponseDto>()
                });
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