using AutoMapper;
using Hangfire;
using WebApplication1.BackgroundJobs.OrderJobs;
using WebApplication1.Constants;
using WebApplication1.DTOS.Request_DTOs;
using WebApplication1.DTOS.Response_DTOs;
using WebApplication1.DTOS.Shared.RequestDto;
using WebApplication1.DTOS.Shared.Response_DTOs;
using WebApplication1.Entitys;
using WebApplication1.Exceptions;
using WebApplication1.PaymentGateway.External.Request;
using WebApplication1.PaymentGateway.Services;
using WebApplication1.Repository.SpecificRepository.AddressRepository;
using WebApplication1.Repository.SpecificRepository.BuyerRepository;
using WebApplication1.Repository.SpecificRepository.CartRepository;
using WebApplication1.Repository.SpecificRepository.CouponRepository;
using WebApplication1.Repository.SpecificRepository.OrderRepository;

namespace WebApplication1.Services.OrderService
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IAddressRepository _addressRepository;
        private readonly ICouponRepository _couponRepository;
        private readonly IBuyerRepository _buyerRepository;
        private readonly IPaymentService _paymentService;
        ISavedCardService _savedCardService;
        private readonly IMapper _mapper;

        public OrderService(
            IOrderRepository orderRepository,
            ICartRepository cartRepository,
            IAddressRepository addressRepository,
            ICouponRepository couponRepository,
            IBuyerRepository buyerRepository,
            ISavedCardService savedCardService,
            IPaymentService paymentService,
            IMapper mapper)
        {
            _savedCardService = savedCardService;
            _buyerRepository = buyerRepository;
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _addressRepository = addressRepository;
            _couponRepository = couponRepository;
            _paymentService = paymentService;
            _mapper = mapper;
        }

        public async Task<ApiResponseDto<OrderResponseDto>> CreateOrderAsync(CreateOrderRequestDto createOrderRequestDto, int buyerId, int userId)
        {
            var Cart = await _cartRepository.GetCartWithItemsAsync(buyerId);
            if (Cart == null || !Cart.Items.Any())
            {
                throw new BadRequestException("Your cart has no items to order.");
            }

            var buyer = await _buyerRepository.GetBuyerByUserId(userId);
            if (buyerId != buyer.BuyerId)
            {
                throw new UnauthorizedException("The provided Buyer ID does not belong to the authenticated user");
            }

            var address = await _addressRepository.GetByIdAsync(createOrderRequestDto.AddressId);
            if (address == null || address.UserId != userId)
            {
                throw new NotFoundException("The specified address was not found or does not belong to you.");
            }
            var SavedCard = await _savedCardService.GetMyCardsAsync(userId);
            if (!SavedCard.Data.Any())
            {
                throw new NotFoundException("You Dont Have any Cards ");
            }

            decimal subtotal = 0;
            foreach (var item in Cart.Items)
            {
                var freeQuantity = item.ProductVariant.QuantityInStock - item.ProductVariant.ReservedQuantity;
                if (freeQuantity < item.Quantity)
                {
                    throw new BadRequestException($"Not enough stock for variant. Requested: {item.Quantity}, Available: {freeQuantity}.");
                }
                subtotal += item.Quantity * item.ProductVariant.Price;
            }

            decimal discountAmount = 0;
            Coupon? appliedCoupon = null;

            if (!string.IsNullOrWhiteSpace(createOrderRequestDto.CouponCode))
            {
                appliedCoupon = await _couponRepository.GetCouponByCodeAsync(createOrderRequestDto.CouponCode);

                if (appliedCoupon == null ||
                    appliedCoupon.StartDate > DateTime.UtcNow ||
                    appliedCoupon.EndDate < DateTime.UtcNow ||
                    (appliedCoupon.UsageLimit.HasValue && appliedCoupon.UsedCount >= appliedCoupon.UsageLimit))
                {
                    throw new BadRequestException("The provided coupon is invalid, expired, or has reached its usage limit.");
                }

                if (appliedCoupon.DiscountType == DiscountType.Percentage)
                {
                    discountAmount = subtotal * (appliedCoupon.DiscountValue / 100);
                }
                else if (appliedCoupon.DiscountType == DiscountType.FixedAmount)
                {
                    discountAmount = appliedCoupon.DiscountValue;
                }
            }

            decimal finalTotal = Math.Max(0, subtotal - discountAmount);

            var order = new Order
            {
                BuyerId = buyer.BuyerId,
                AddressId = address.AddressId,
                CreatedAt = DateTime.UtcNow,
                TotalAmount = finalTotal,
                DiscountAmount = discountAmount,
                CouponId = appliedCoupon?.CouponId,
                Status = OrderStatus.Pending,
                IsDeleted = false,
                OrderItems = Cart.Items.Select(ci => new OrderItem
                {
                    ProductVariantId = ci.ProductVariantId,
                    Quantity = ci.Quantity,
                    Price = ci.ProductVariant.Price,
                    ProductVariant = ci.ProductVariant,
                }).ToList()
            };

            await _orderRepository.AddAsync(order);

            foreach (var item in Cart.Items)
            {
                item.ProductVariant.QuantityInStock -= item.Quantity;

                if (item.ProductVariant.ReservedQuantity >= item.Quantity)
                {
                    item.ProductVariant.ReservedQuantity -= item.Quantity;
                }
            }

            if (appliedCoupon != null)
            {
                appliedCoupon.UsedCount = (appliedCoupon.UsedCount ?? 0) + 1;
                _couponRepository.Update(appliedCoupon);
            }

            _cartRepository.Delete(Cart);

            await _orderRepository.SaveChangesAsync();

            BackgroundJob.Schedule<IOrderBackgroundJobs>(
                job => job.CheckAndCancelUnpaidOrderAsyn(order.OrderId),
                TimeSpan.FromMinutes(30));

            var PaymentRequest = new PaymentRequestDto
            {
                CustomerEmail = buyer.User.Email,
                CurrencyCode = createOrderRequestDto.CurrencyCode,
                OrderId = order.OrderId,
                CustomerName = buyer.User.UserName,
                TotalAmount = finalTotal,
            };
            var InitializePaymentResponse = await _paymentService.InitializePayment(PaymentRequest);
            if (InitializePaymentResponse.IsSuccess == false)
            {
                throw new BadRequestException("لرابط الخارجي غير صحيح أو السيرفر غير متاح" +
                    "The payment has been decliend Order Status is Pending For 24 hours");
                
            }
            var orderResponse = _mapper.Map<OrderResponseDto>(order);
            orderResponse.CheckoutUrl = InitializePaymentResponse.CheckoutUrl;
            return new ApiResponseDto<OrderResponseDto>
            {

                Message = "Order created successfully.",
                Data = orderResponse
            };
        }
        public async Task<ApiResponseDto<PaginatedResponseDto<OrderResponseDto>>> GetOrdersByBuyerIdAsync(int buyerId, int userId, PaginationRequestDto paginationRequestDto)
        {
            var buyer = await _buyerRepository.GetBuyerByUserId(userId);
            if (buyer == null || buyer.BuyerId != buyerId)
            {
                throw new UnauthorizedException("The provided Buyer ID does not belong to the authenticated user.");
            }

            var (orders, totalCount) = await _orderRepository.GetOrdersListByBuyerIdAsync(
                buyerId,
                paginationRequestDto.PageNumber,
                paginationRequestDto.PageSize
            );

            int totalPages = (int)Math.Ceiling(totalCount / (double)paginationRequestDto.PageSize);

            var mappedOrders = _mapper.Map<List<OrderResponseDto>>(orders);

            return new ApiResponseDto<PaginatedResponseDto<OrderResponseDto>>
            {
                Message = "Orders retrieved successfully.",
                Data = new PaginatedResponseDto<OrderResponseDto>
                {
                    CurrentPage = paginationRequestDto.PageNumber,
                    PageSize = paginationRequestDto.PageSize,
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    Data = mappedOrders!
                }
            };
        }

        public async Task<ApiResponseDto<OrderResponseDto>> GetOrderByIdAsync(int orderId, int userId)
        {
            var buyer = await _buyerRepository.GetBuyerByUserId(userId);
            if (buyer == null)
            {
                throw new BadRequestException("User profile not found.");
            }

            var order = await _orderRepository.GetOrderWithDetailsAsync(orderId, buyer.BuyerId);

            if (order == null || order.IsDeleted)
            {
                throw new NotFoundException("Order not found or you do not have permission to view it.");
            }

            return new ApiResponseDto<OrderResponseDto>
            {
                Message = "Order retrieved successfully.",
                Data = _mapper.Map<OrderResponseDto>(order)
            };
        }

        public async Task<ApiResponseDto<string>> UpdateOrderStatusAsync(int orderId, string newStatus)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);

            if (order == null || order.IsDeleted)
            {
                throw new NotFoundException("The specified order does not exist.");
            }

            if (newStatus != OrderStatus.successful && newStatus != OrderStatus.cancelled && newStatus != OrderStatus.Pending)
            {
                throw new NotFoundException("The specified status does not exist. ");
            }
            order.Status = newStatus;
            _orderRepository.Update(order);
            await _orderRepository.SaveChangesAsync();

            return new ApiResponseDto<string>
            {
                Message = $"Order status updated to {newStatus} successfully.",
                Data = null
            };
        }

        public async Task<ApiResponseDto<string>> CancelOrderAsync(int orderId, int userId)
        {
            var buyer = await _buyerRepository.GetBuyerByUserId(userId);
            if (buyer == null)
            {
                throw new NotFoundException("User profile not found.");
            }

            var order = await _orderRepository.GetOrderWithDetailsAsync(orderId, buyer.BuyerId);

            if (order == null || order.IsDeleted)
            {
                throw new NotFoundException("Order not found.");
            }

            if (order.Status == OrderStatus.cancelled)
            {
                throw new BadRequestException($"Order cannot be cancelled because it is already {order.Status}.");
            }

            order.Status = OrderStatus.cancelled;

            foreach (var item in order.OrderItems)
            {
                if (item.ProductVariant != null)
                {
                    item.ProductVariant.QuantityInStock += item.Quantity;
                }
            }

            if (order.CouponId.HasValue)
            {
                var coupon = await _couponRepository.GetByIdAsync(order.CouponId.Value);
                if (coupon != null && coupon.UsedCount > 0)
                {
                    coupon.UsedCount -= 1;
                    _couponRepository.Update(coupon);
                }
            }

            _orderRepository.Update(order);
            await _orderRepository.SaveChangesAsync();

            return new ApiResponseDto<string>
            {
                Message = "Order cancelled successfully and stock has been restored.",
                Data = null
            };
        }

        public async Task<ApiResponseDto<string>> HandlePaymentCallbackAsync(int orderId, bool isSuccess)
        {
            var order = await _orderRepository.GetOrderWithItemsByIdAsync(orderId);

            if (order == null || order.IsDeleted)
            {
                throw new NotFoundException("Order not found.");
            }
            if (order.Status == OrderStatus.cancelled)
            {
                throw new BadRequestException("This order is already cancelled");
            }
            if(order.Status == OrderStatus.successful)
            {
                throw new BadRequestException("This order has already been payed");
            }

            if (isSuccess)
            {
                order.Status = OrderStatus.successful;
            }
            else
            {
                order.Status = OrderStatus.cancelled;

                foreach (var item in order.OrderItems)
                {
                    if (item.ProductVariant != null)
                    {
                        item.ProductVariant.QuantityInStock += item.Quantity;
                    }
                }

                if (order.CouponId.HasValue)
                {
                    var coupon = await _couponRepository.GetByIdAsync(order.CouponId.Value);
                    if (coupon != null && coupon.UsedCount > 0)
                    {
                        coupon.UsedCount -= 1;
                        _couponRepository.Update(coupon);
                    }
                }
            }

            _orderRepository.Update(order);
            await _orderRepository.SaveChangesAsync();

            return new ApiResponseDto<string>
            {
                Message = isSuccess ? "Payment successful. Order is processing." : "Payment failed. Order cancelled and stock restored.",
                Data = null
            };
        }
    }
}