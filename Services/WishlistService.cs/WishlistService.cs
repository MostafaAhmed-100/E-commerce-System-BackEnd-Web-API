using AutoMapper;
using Microsoft.Extensions.Logging;
using WebApplication1.DTOS.Request_DTOs;
using WebApplication1.DTOS.Response_DTOs;
using WebApplication1.DTOS.Shared.RequestDto;
using WebApplication1.DTOS.Shared.Response_DTOs;
using WebApplication1.Entitys;
using WebApplication1.Exceptions;
using WebApplication1.Repository.UnitOfWork;
using WebApplication1.Services.WishlistService.cs;

namespace WebApplication1.Services.WishlistService
{
    public class WishlistService : IWishlistService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<WishlistService> _logger;

        public WishlistService
        (
           IUnitOfWork unitOfWork,
           IMapper mapper,
           ILogger<WishlistService> logger
        )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResponseDto<string>> CreateWishlistAsync(int buyerId, CreateWishlistRequestDto createWishlistRequest)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var buyer = await _unitOfWork.BuyerRepository.GetByIdAsync(buyerId);
                if (buyer == null)
                {
                    _logger.LogWarning("Buyer {BuyerId} not found while creating wishlist.", buyerId);
                    throw new NotFoundException("Buyer not found.");
                }

                var newWishlist = new Wishlist
                {
                    BuyerId = buyerId,
                    WishlistName = createWishlistRequest.WishlistName,
                    Buyer = buyer,
                    CreatedAt = DateTime.UtcNow,
                };

                await _unitOfWork.WishlistsRepository.AddAsync(newWishlist);
                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Wishlist {WishlistName} created successfully for Buyer {BuyerId}.", createWishlistRequest.WishlistName, buyerId);

                return new ApiResponseDto<string>
                {
                    Message = "Wishlist created successfully.",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error occurred while creating wishlist for Buyer {BuyerId}", buyerId);
                throw;
            }
        }

        public async Task<ApiResponseDto<IEnumerable<WishlistDto>>> GetBuyerWishlistsAsync(int buyerId)
        {
            try
            {
                var wishlists = await _unitOfWork.WishlistsRepository.GetBuyerWishlistsAsync(buyerId);

                var wishlistsDto = _mapper.Map<IEnumerable<WishlistDto>>(wishlists);

                return new ApiResponseDto<IEnumerable<WishlistDto>>
                {
                    Message = "Wishlists retrieved successfully.",
                    Data = wishlistsDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving wishlists for Buyer {BuyerId}", buyerId);
                throw;
            }
        }

        public async Task<ApiResponseDto<string>> DeleteWishlistAsync(int wishlistId)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var wishlist = await _unitOfWork.WishlistsRepository.GetByIdAsync(wishlistId);
                if (wishlist == null)
                {
                    _logger.LogWarning("Wishlist {WishlistId} not found for deletion.", wishlistId);
                    throw new NotFoundException("Wishlist not found.");
                }

                _unitOfWork.WishlistsRepository.Delete(wishlist);
                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Wishlist {WishlistId} deleted successfully.", wishlistId);

                return new ApiResponseDto<string>
                {
                    Message = "Wishlist deleted successfully.",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error occurred while deleting Wishlist {WishlistId}", wishlistId);
                throw;
            }
        }

        public async Task<ApiResponseDto<string>> ToggleWishlistItemAsync(int wishlistId, int variantId)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var wishlist = await _unitOfWork.WishlistsRepository.GetByIdAsync(wishlistId);
                if (wishlist == null)
                {
                    throw new NotFoundException("Wishlist not found.");
                }

                bool exists = await _unitOfWork.WishlistItemRepository.CheckVariantExistsInWishlistAsync(wishlistId, variantId);

                if (exists)
                {
                    var items = await _unitOfWork.WishlistItemRepository.FindAsync(w => w.WishlistId == wishlistId && w.productVariantId == variantId);
                    var itemToRemove = items.FirstOrDefault();

                    if (itemToRemove != null)
                    {
                        _unitOfWork.WishlistItemRepository.Delete(itemToRemove);
                        await _unitOfWork.SaveChangesAsync();
                        await transaction.CommitAsync();

                        _logger.LogInformation("Variant {VariantId} removed from Wishlist {WishlistId}.", variantId, wishlistId);

                        return new ApiResponseDto<string>
                        {
                            Message = "Item removed from wishlist.",
                            Data = null
                        };
                    }
                }

                var newItem = new WishlistItem
                {
                    WishlistId = wishlistId,
                    productVariantId = variantId
                };

                await _unitOfWork.WishlistItemRepository.AddAsync(newItem);
                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Variant {VariantId} added to Wishlist {WishlistId}.", variantId, wishlistId);

                return new ApiResponseDto<string>
                {
                    Message = "Item added to wishlist.",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error occurred while toggling variant {VariantId} in Wishlist {WishlistId}", variantId, wishlistId);
                throw;
            }
        }

        public async Task<ApiResponseDto<PaginatedResponseDto<WishlistItemResponseDto>>> GetWishlistItemsAsync(int wishlistId, PaginationRequestDto paginationRequest)
        {
            try
            {
                var result = await _unitOfWork.WishlistItemRepository.GetWishlistItemsPaginatedAsync(wishlistId, paginationRequest.PageNumber, paginationRequest.PageSize);

                var itemsDto = _mapper.Map<IEnumerable<WishlistItemResponseDto>>(result.Items).ToList();

                int totalPages = (int)Math.Ceiling(result.TotalCount / (double)paginationRequest.PageSize);

                var paginatedResponse = new PaginatedResponseDto<WishlistItemResponseDto>
                {
                    Data = itemsDto,
                    CurrentPage = paginationRequest.PageNumber,
                    PageSize = paginationRequest.PageSize,
                    TotalCount = result.TotalCount,
                    TotalPages = totalPages
                };

                return new ApiResponseDto<PaginatedResponseDto<WishlistItemResponseDto>>
                {
                    Message = "Wishlist items retrieved successfully.",
                    Data = paginatedResponse
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving items for Wishlist {WishlistId}", wishlistId);
                throw;
            }
        }
    }
}