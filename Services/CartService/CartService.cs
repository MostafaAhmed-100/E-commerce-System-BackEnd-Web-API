using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.DTOS.Request_DTOs;
using WebApplication1.DTOS.Response_DTOs;
using WebApplication1.DTOS.Shared.Response_DTOs;
using WebApplication1.Entitys;
using WebApplication1.Repository.GenericRepository;
using WebApplication1.Repository.SpecificRepository.CartRepository;
using WebApplication1.Services.CartService;
using WebApplication1.Services.Interface;

namespace WebApplication1.Services.Implementation
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IGenericRepository<ProductVariant> _variantRepository;
        private readonly IMapper _mapper;

        public CartService(ICartRepository cartRepository, IGenericRepository<ProductVariant> variantRepository, IMapper mapper)
        {
            _cartRepository = cartRepository;
            _variantRepository = variantRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponseDto<CartResponseDto>> GetCartBybuyerId(int buyerId)
        {
            var cart = await _cartRepository.GetCartWithItemsAsync(buyerId);

            if (cart == null)
            {
                return new ApiResponseDto<CartResponseDto>
                {
                    IsSuccess = true,
                    StatusCode = 200,
                    ErrorCode = "",
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
                IsSuccess = true,
                StatusCode = 200,
                ErrorCode = "",
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
                return new ApiResponseDto<CartResponseDto>
                {
                    IsSuccess = false,
                    StatusCode = 404,
                    ErrorCode = "PRODUCT_NOT_FOUND",
                    Data = null,
                    Message = "The product does not exist"
                };
            }
            if (variant.QuantityInStock < addToCartRequestDto.Quantity)
            {
                return new ApiResponseDto<CartResponseDto>
                {
                    IsSuccess = false,
                    StatusCode = 400,
                    ErrorCode = "INSUFFICIENT_QUANTITY",
                    Data = null,
                    Message = "The Quantity Request is insufficient"
                };
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

            if (cart.Items.Any(i => i.ProductVariantId == addToCartRequestDto.ProductVariantId))
            {
                var existingItem = cart.Items.First(i => i.ProductVariantId == addToCartRequestDto.ProductVariantId);

                if (existingItem.Quantity + addToCartRequestDto.Quantity > variant.QuantityInStock)
                {
                    return new ApiResponseDto<CartResponseDto>
                    {
                        IsSuccess = false,
                        StatusCode = 400,
                        ErrorCode = "INSUFFICIENT_QUANTITY",
                        Data = null,
                        Message = "Insufficient quantity in stock for the total amount"
                    };
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
            return await GetCartBybuyerId(buyerId);
        }

        public async Task<ApiResponseDto<CartResponseDto>> UpdateItemQuantity(int buyerId, int variantId, UpdateCartItemQuantityRequestDto UpdateCartItemQuantityRequestDto)
        {
            var cart = await _cartRepository.GetCartWithItemsAsync(buyerId);
            if (cart == null)
            {
                return new ApiResponseDto<CartResponseDto>
                {
                    IsSuccess = false,
                    Data = null,
                    ErrorCode = "CART_NOT_FOUND",
                    StatusCode = 404,
                    Message = "This buyer has no Cart"
                };
            }

            var Item = cart.Items.FirstOrDefault(v => v.ProductVariantId == variantId);
            if (Item == null)
            {
                return new ApiResponseDto<CartResponseDto>
                {
                    IsSuccess = false,
                    Data = null,
                    ErrorCode = "ITEM_NOT_IN_CART",
                    StatusCode = 404,
                    Message = "That item Does not exist in The Cart"
                };
            }

            var ItemQuantity = await _variantRepository.GetByIdAsync(variantId);
            if (ItemQuantity == null)
            {
                return new ApiResponseDto<CartResponseDto>
                {
                    IsSuccess = false,
                    Data = null,
                    ErrorCode = "item_NOT_FOUND",
                    StatusCode = 404,
                    Message = "That item Does not exist in The DataBase"
                };
            }

            var freeQuantity = ItemQuantity.QuantityInStock - ItemQuantity.ReservedQuantity;
            if (freeQuantity < UpdateCartItemQuantityRequestDto.Quantity)
            {
                return new ApiResponseDto<CartResponseDto>
                {
                    IsSuccess = false,
                    Data = null,
                    ErrorCode = "INSUFFICIENT_QUANTITY",
                    StatusCode = 400,
                    Message = "The Quantity Ubdated Is More Than In Stock"
                };
            }

            Item.Quantity = UpdateCartItemQuantityRequestDto.Quantity;
            await _cartRepository.SaveChangesAsync();
            return await GetCartBybuyerId(buyerId);
        }

        public async Task<ApiResponseDto<CartResponseDto>> RemoveFromCart(int buyerId, int variantId)
        {
            var cart = await _cartRepository.GetCartWithItemsAsync(buyerId);
            if (cart == null)
            {
                return new ApiResponseDto<CartResponseDto>
                {
                    IsSuccess = false,
                    Data = null,
                    ErrorCode = "CART_NOT_FOUND",
                    StatusCode = 404,
                    Message = "This buyer has no Cart"
                };
            }

            var Item = cart.Items.FirstOrDefault(v => v.ProductVariantId == variantId);
            if (Item == null)
            {
                return new ApiResponseDto<CartResponseDto>
                {
                    IsSuccess = false,
                    Data = null,
                    ErrorCode = "ITEM_NOT_IN_CART",
                    StatusCode = 404,
                    Message = "That item Does not exist in The Cart"
                };
            }

            cart.Items.Remove(Item);
            await _cartRepository.SaveChangesAsync();
            return await GetCartBybuyerId(buyerId);
        }

        public async Task<ApiResponseDto<CartResponseDto>> ClearCart(int buyerId)
        {
            var cart = await _cartRepository.GetCartWithItemsAsync(buyerId);
            if (cart == null)
            {
                return new ApiResponseDto<CartResponseDto>
                {
                    IsSuccess = true,
                    StatusCode = 200,
                    ErrorCode = "",
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
            return await GetCartBybuyerId(buyerId);
        }
    }
}