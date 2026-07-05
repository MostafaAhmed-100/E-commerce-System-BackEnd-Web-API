using AutoMapper;
using Microsoft.Extensions.Logging;
using WebApplication1.DTOS.Request_DTOs;
using WebApplication1.DTOS.Response_DTOs;
using WebApplication1.Entitys;
using WebApplication1.Exceptions;
using WebApplication1.Repository.GenericRepository;
using WebApplication1.Repository.SpecificRepository.CartRepository;
using WebApplication1.Services.CartService;

namespace WebApplication1.Services.Implementation
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IGenericRepository<ProductVariant> _variantRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CartService> _logger;

        public CartService(ICartRepository cartRepository,
            IGenericRepository<ProductVariant> variantRepository,
            IMapper mapper,
            ILogger<CartService> logger)
        {
            _cartRepository = cartRepository;
            _variantRepository = variantRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResponseDto<CartResponseDto>> GetCartBybuyerId(int buyerId)
        {
            var cart = await _cartRepository.GetCartWithItemsAsync(buyerId);
            if (cart == null)
            {
                return new ApiResponseDto<CartResponseDto>
                {
                    Data = new CartResponseDto
                    {
                        CartId = 0,
                        Items = new List<CartItemResponseDto>(),
                        TotalPrice = 0
                    },
                    Message = "The Cart Has been Created (Empty)"
                };
            }
            var mappedItems = _mapper.Map<List<CartItemResponseDto>>(cart.Items);
            decimal totalPrice = mappedItems.Sum(item => item.SubTotal);

            return new ApiResponseDto<CartResponseDto>
            {
                Data = new CartResponseDto
                {
                    CartId = cart.CartId,
                    Items = mappedItems,
                    TotalPrice = totalPrice
                },
                Message = $"This is the cart for the user {buyerId}"
            };
        }

        public async Task<ApiResponseDto<CartResponseDto>> AddToCart(int buyerId, AddToCartRequestDto addToCartRequestDto)
        {
            var variant = await _variantRepository.GetByIdAsync(addToCartRequestDto.ProductVariantId);
            if (variant == null)
            {
                _logger.LogWarning("Buyer {BuyerId} attempted to add non-existent ProductVariant {ProductVariantId} to cart.", buyerId, addToCartRequestDto.ProductVariantId);
                throw new NotFoundException("The product does not exist");
            }

            if (variant.QuantityInStock < addToCartRequestDto.Quantity)
            {
                _logger.LogWarning("Buyer {BuyerId} requested Quantity {RequestedQuantity} " +
                    "for ProductVariant {ProductVariantId}, but only {QuantityInStock} is available.",
                    buyerId, addToCartRequestDto.Quantity, addToCartRequestDto.ProductVariantId,
                    variant.QuantityInStock);
                throw new BadRequestException("The Quantity Request is insufficient");
            }

            var cart = await _cartRepository.GetCartWithItemsAsync(buyerId);

            if (cart == null)
            {
                cart = new Cart
                {
                    BuyerId = buyerId,
                    Items = new List<CartItem>()
                };
                await _cartRepository.AddAsync(cart);
            }

            var existingItem = cart.Items.FirstOrDefault(i => i.ProductVariantId == addToCartRequestDto.ProductVariantId);

            if (existingItem != null)
            {
                if (existingItem.Quantity + addToCartRequestDto.Quantity > variant.QuantityInStock)
                {
                    _logger.LogWarning("Buyer {BuyerId} attempted to increase Quantity for ProductVariant {ProductVariantId} by {RequestedQuantity}," +
                        " but total exceeds available stock {QuantityInStock}.", buyerId,
                        addToCartRequestDto.ProductVariantId, addToCartRequestDto.Quantity, variant.QuantityInStock);
                    throw new BadRequestException("Insufficient quantity in stock for the total amount");
                }
                existingItem.Quantity += addToCartRequestDto.Quantity;
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    ProductVariantId = addToCartRequestDto.ProductVariantId,
                    Quantity = addToCartRequestDto.Quantity
                });
            }

            await _cartRepository.SaveChangesAsync();
            _logger.LogInformation("Buyer {BuyerId} successfully added Quantity {Quantity} " +
                "of ProductVariant {ProductVariantId} to cart.", buyerId, addToCartRequestDto.Quantity, addToCartRequestDto.ProductVariantId);

            return await GetCartBybuyerId(buyerId);
        }

        public async Task<ApiResponseDto<CartResponseDto>> UpdateItemQuantity(int buyerId, int variantId, UpdateCartItemQuantityRequestDto UpdateCartItemQuantityRequestDto)
        {
            var cart = await _cartRepository.GetCartWithItemsAsync(buyerId);

            if (cart == null)
            {
                _logger.LogWarning("Buyer {BuyerId} attempted to update item quantity but has no active cart.", buyerId);
                throw new NotFoundException("This buyer has no Cart");
            }

            var Item = cart.Items.FirstOrDefault(v => v.ProductVariantId == variantId);
            if (Item == null)
            {
                _logger.LogWarning("Buyer {BuyerId} attempted to update quantity for ProductVariant {VariantId} which is not in their cart.", buyerId, variantId);
                throw new NotFoundException("That item Does not exist in The Cart");
            }

            var ItemQuantity = await _variantRepository.GetByIdAsync(variantId);
            if (ItemQuantity == null)
            {
                _logger.LogWarning("Buyer {BuyerId} attempted to update quantity for ProductVariant {VariantId} which no longer exists in DB.", buyerId, variantId);
                throw new NotFoundException("That item Does not exist in The DataBase");
            }

            var freeQuantity = ItemQuantity.QuantityInStock - ItemQuantity.ReservedQuantity;
            if (freeQuantity < UpdateCartItemQuantityRequestDto.Quantity)
            {
                _logger.LogWarning("Buyer {BuyerId} requested update" +
                    " to Quantity {RequestedQuantity} for ProductVariant {VariantId}, " +
                    "but only {FreeQuantity} is free in stock.", buyerId, UpdateCartItemQuantityRequestDto.Quantity, variantId, freeQuantity);
                throw new BadRequestException("The Quantity Ubdated Is More Than In Stock");
            }

            Item.Quantity = UpdateCartItemQuantityRequestDto.Quantity;
            await _cartRepository.SaveChangesAsync();

            _logger.LogInformation("Buyer {BuyerId} successfully" +
                " updated quantity for ProductVariant {VariantId} to {NewQuantity}.", buyerId, variantId, UpdateCartItemQuantityRequestDto.Quantity);

            return await GetCartBybuyerId(buyerId);
        }

        public async Task<ApiResponseDto<CartResponseDto>> RemoveFromCart(int buyerId, int variantId)
        {
            var cart = await _cartRepository.GetCartWithItemsAsync(buyerId);
            if (cart == null)
            {
                _logger.LogWarning("Buyer {BuyerId} attempted to remove an item but has no active cart.", buyerId);
                throw new NotFoundException("This buyer has no Cart");
            }

            var Item = cart.Items.FirstOrDefault(v => v.ProductVariantId == variantId);
            if (Item == null)
            {
                _logger.LogWarning("Buyer {BuyerId} attempted to remove ProductVariant {VariantId} which is not in their cart.", buyerId, variantId);
                throw new NotFoundException("That item Does not exist in The Cart");
            }

            cart.Items.Remove(Item);
            await _cartRepository.SaveChangesAsync();

            _logger.LogInformation("Buyer {BuyerId} successfully removed ProductVariant {VariantId} from their cart.", buyerId, variantId);

            return await GetCartBybuyerId(buyerId);
        }

        public async Task<ApiResponseDto<CartResponseDto>> ClearCart(int buyerId)
        {
            var cart = await _cartRepository.GetCartWithItemsAsync(buyerId);
            if (cart == null)
            {
                return new ApiResponseDto<CartResponseDto>
                {
                    Data = new CartResponseDto
                    {
                        CartId = 0,
                        Items = new List<CartItemResponseDto>(),
                        TotalPrice = 0
                    },
                    Message = "The Cart Has been Created (Empty)"
                };
            }

            cart.Items.Clear();
            await _cartRepository.SaveChangesAsync();

            _logger.LogInformation("Buyer {BuyerId} successfully cleared their entire cart.", buyerId);

            return await GetCartBybuyerId(buyerId);
        }
    }
}