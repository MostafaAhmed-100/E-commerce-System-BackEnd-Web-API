using AutoMapper;
using Microsoft.Extensions.Logging;
using WebApplication1.DTOS.Request_DTOs;
using WebApplication1.DTOS.Response_DTOs;
using WebApplication1.DTOS.Shared.RequestDto;
using WebApplication1.DTOS.Shared.Response_DTOs;
using WebApplication1.Entitys;
using WebApplication1.Exceptions;
using WebApplication1.Repository.SpecificRepository.BuyerRepository;
using WebApplication1.Repository.SpecificRepository.WishlistItemRepository;
using WebApplication1.Repository.SpecificRepository.WishlistsRepository;
using WebApplication1.Services.WishlistService.cs;

namespace WebApplication1.Services.WishlistService
{
    public class WishlistService : IWishlistService
    {
        private readonly IWishlistsRepository _wishlistsRepository;
        private readonly IWishlistItemRepository _wishlistItemRepository;
        private readonly IBuyerRepository _buyerRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<WishlistService> _logger;

        public WishlistService
        (
           IWishlistItemRepository wishlistItemRepository,
           IWishlistsRepository wishlistsRepository,
           IBuyerRepository buyerRepository,
           IMapper mapper,
           ILogger<WishlistService> logger
        )
        {
            _wishlistItemRepository = wishlistItemRepository;
            _wishlistsRepository = wishlistsRepository;
            _buyerRepository = buyerRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResponseDto<string>> CreateWishlistAsync(int buyerId, CreateWishlistRequestDto createWishlistRequest)
        {
            var buyer = await _buyerRepository.GetByIdAsync(buyerId);
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
                CreatedAt= DateTime.UtcNow,
            };

            await _wishlistsRepository.AddAsync(newWishlist);
            await _wishlistsRepository.SaveChangesAsync();

            _logger.LogInformation("Wishlist {WishlistName} created successfully for Buyer {BuyerId}.", createWishlistRequest.WishlistName, buyerId);

            return new ApiResponseDto<string>
            {
                Message = "Wishlist created successfully.",
                Data = null
            };
        }

        public async Task<ApiResponseDto<IEnumerable<WishlistDto>>> GetBuyerWishlistsAsync(int buyerId)
        {
            var wishlists = await _wishlistsRepository.GetBuyerWishlistsAsync(buyerId);

            var wishlistsDto = _mapper.Map<IEnumerable<WishlistDto>>(wishlists);

            return new ApiResponseDto<IEnumerable<WishlistDto>>
            {
                Message = "Wishlists retrieved successfully.",
                Data = wishlistsDto
            };
        }

        public async Task<ApiResponseDto<string>> DeleteWishlistAsync(int wishlistId)
        {
            var wishlist = await _wishlistsRepository.GetByIdAsync(wishlistId);
            if (wishlist == null)
            {
                _logger.LogWarning("Wishlist {WishlistId} not found for deletion.", wishlistId);
                throw new NotFoundException("Wishlist not found.");
            }

            _wishlistsRepository.Delete(wishlist);
            await _wishlistsRepository.SaveChangesAsync();

            _logger.LogInformation("Wishlist {WishlistId} deleted successfully.", wishlistId);

            return new ApiResponseDto<string>
            {
                Message = "Wishlist deleted successfully.",
                Data = null
            };
        }

        public async Task<ApiResponseDto<string>> ToggleWishlistItemAsync(int wishlistId, int variantId)
        {
            var wishlist = await _wishlistsRepository.GetByIdAsync(wishlistId);
            if (wishlist == null)
            {
                throw new NotFoundException("Wishlist not found.");
            }

            bool exists = await _wishlistItemRepository.CheckVariantExistsInWishlistAsync(wishlistId, variantId);

            if (exists)
            {
                var items = await _wishlistItemRepository.FindAsync(w => w.WishlistId == wishlistId && w.productVariantId == variantId);
                var itemToRemove = items.FirstOrDefault();

                if (itemToRemove != null)
                {
                    _wishlistItemRepository.Delete(itemToRemove);
                    await _wishlistItemRepository.SaveChangesAsync();
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

            await _wishlistItemRepository.AddAsync(newItem);
            await _wishlistItemRepository.SaveChangesAsync();

            _logger.LogInformation("Variant {VariantId} added to Wishlist {WishlistId}.", variantId, wishlistId);

            return new ApiResponseDto<string>
            {
                Message = "Item added to wishlist.",
                Data = null
            };
        }

        public async Task<ApiResponseDto<PaginatedResponseDto<WishlistItemResponseDto>>> GetWishlistItemsAsync(int wishlistId, PaginationRequestDto paginationRequest)
        {
            var result = await _wishlistItemRepository.GetWishlistItemsPaginatedAsync(wishlistId, paginationRequest.PageNumber, paginationRequest.PageSize);

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
    }
}