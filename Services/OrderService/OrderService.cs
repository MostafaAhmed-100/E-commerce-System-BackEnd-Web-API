using AutoMapper;
using Hangfire;
using Microsoft.Extensions.Logging;
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
using WebApplication1.Repository.SpecificRepository.LoyaltyTransactionRepository;
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
        private readonly ISavedCardService _savedCardService;
        private readonly ILoyaltyTransactionRepository _loyaltyTransactionRepository;
        private readonly ILogger<OrderService> _logger;
        private readonly IMapper _mapper;

        public OrderService(
            ILogger<OrderService> logger,
            IOrderRepository orderRepository,
            ICartRepository cartRepository,
            IAddressRepository addressRepository,
            ICouponRepository couponRepository,
            IBuyerRepository buyerRepository,
            ISavedCardService savedCardService,
            IPaymentService paymentService,
            ILoyaltyTransactionRepository loyaltyTransactionRepository,
            IMapper mapper)
        {
            _logger = logger;
            _savedCardService = savedCardService;
            _buyerRepository = buyerRepository;
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _addressRepository = addressRepository;
            _couponRepository = couponRepository;
            _paymentService = paymentService;
            _loyaltyTransactionRepository = loyaltyTransactionRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponseDto<OrderResponseDto>> CreateOrderAsync(CreateOrderRequestDto createOrderRequestDto, int buyerId, int userId)
        {
            var Cart = await _cartRepository.GetCartWithItemsAsync(buyerId);
            if (Cart == null || !Cart.Items.Any())
            {
                _logger.LogWarning("User {UserId} attempted to create an order but cart is empty for Buyer {BuyerId}.", userId, buyerId);
                throw new BadRequestException("Your cart has no items to order.");
            }

            var buyer = await _buyerRepository.GetBuyerByUserId(userId);
            if (buyerId != buyer.BuyerId)
            {
                _logger.LogWarning("Security Warning: User {UserId} attempted to create an order for unauthorized BuyerId {AttemptedBuyerId}.", userId, buyerId);
                throw new UnauthorizedException("The provided Buyer ID does not belong to the authenticated user");
            }

            bool isRedeemingPoints = createOrderRequestDto.UseLoyaltyPoints == true && createOrderRequestDto.PointsToRedeem != null;

            if (isRedeemingPoints)
            {
                var points = await _loyaltyTransactionRepository.GetTotalPointsFromLedgerByBuyerIdAsync(buyerId);
                if (points < createOrderRequestDto.PointsToRedeem)
                {
                    _logger.LogWarning("The User {UserId} attempted to use points {PointsToRedeem} he doesn't have, he has {Points}",
                        userId, createOrderRequestDto.PointsToRedeem, points);
                    throw new BadRequestException($"You cant redeem {createOrderRequestDto.PointsToRedeem} points, you only have {points}");
                }
            }

            var address = await _addressRepository.GetByIdAsync(createOrderRequestDto.AddressId);
            if (address == null || address.UserId != userId)
            {
                _logger.LogWarning("User {UserId} provided an invalid or unauthorized AddressId {AddressId}.", userId, createOrderRequestDto.AddressId);
                throw new NotFoundException("The specified address was not found or does not belong to you.");
            }

            var SavedCard = await _savedCardService.GetMyCardsAsync(userId);
            if (!SavedCard.Data.Any())
            {
                _logger.LogWarning("User {UserId} attempted to create an order without any saved cards.", userId);
                throw new NotFoundException("You Dont Have any Cards ");
            }

            decimal subtotal = 0;
            foreach (var item in Cart.Items)
            {
                var freeQuantity = item.ProductVariant.QuantityInStock - item.ProductVariant.ReservedQuantity;
                if (freeQuantity < item.Quantity)
                {
                    _logger.LogWarning("User {UserId} tried to order {Quantity} of ProductVariant {VariantId} but only {Available} is available.", userId, item.Quantity, item.ProductVariantId, freeQuantity);
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
                    _logger.LogWarning("User {UserId} attempted to use invalid, expired, or depleted coupon {CouponCode}.", userId, createOrderRequestDto.CouponCode);
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

                _logger.LogInformation("User {UserId} successfully applied coupon {CouponCode} with discount amount {DiscountAmount}.", userId, appliedCoupon.CouponCode, discountAmount);
            }

            decimal finalTotal = Math.Max(0, subtotal - discountAmount);

            if (isRedeemingPoints)
            {
                if (createOrderRequestDto.PointsToRedeem / 100 < 100)
                {
                    _logger.LogWarning("The User {UserId} attempted to redeem points that equals less than 100 EGP: points {PointsToRedeem} equals {Pounds} EGP",
                        userId, createOrderRequestDto.PointsToRedeem, (createOrderRequestDto.PointsToRedeem / 100));
                    throw new BadRequestException("Your points to redeem equals less than 100 EGP.");
                }
                finalTotal = UsePoints(finalTotal, (int)createOrderRequestDto.PointsToRedeem);
            }

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

            if (isRedeemingPoints)
            {
                buyer.LoyaltyPoints = (buyer.LoyaltyPoints ?? 0) - (int)createOrderRequestDto.PointsToRedeem;
                _buyerRepository.Update(buyer);

                var loyaltyTransaction = new LoyaltyTransaction
                {
                    CreatedAt = DateTime.UtcNow,
                    BuyerId = buyerId,
                    OrderId = order.OrderId, 
                    TransactionType = TransactionType.Redeemed,
                    Points = -(int)createOrderRequestDto.PointsToRedeem, 
                };
                await _loyaltyTransactionRepository.AddAsync(loyaltyTransaction);
                await _loyaltyTransactionRepository.SaveChangesAsync();
            }

            _logger.LogInformation("Order {OrderId} successfully created for Buyer {BuyerId} (User {UserId}) with TotalAmount {TotalAmount}.", order.OrderId, buyer.BuyerId, userId, finalTotal);

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
                _logger.LogWarning("Payment initialization failed for Order {OrderId} belonging to User {UserId}.", order.OrderId, userId);
                throw new BadRequestException("الرابط الخارجي غير صحيح أو السيرفر غير متاح. The payment has been declined. Order Status is Pending.");
            }

            _logger.LogInformation("Payment successfully initialized for Order {OrderId}. Checkout URL generated.", order.OrderId);

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
                _logger.LogWarning("Security Warning: User {UserId} attempted to view orders for unauthorized BuyerId {AttemptedBuyerId}.", userId, buyerId);
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
                _logger.LogWarning("User {UserId} requested Order {OrderId} which was not found or is unauthorized.", userId, orderId);
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
                _logger.LogWarning("Attempted to update status for non-existent Order {OrderId}.", orderId);
                throw new NotFoundException("The specified order does not exist.");
            }

            if (newStatus != OrderStatus.successful && newStatus != OrderStatus.cancelled && newStatus != OrderStatus.Pending)
            {
                _logger.LogWarning("Attempted to update Order {OrderId} to invalid status {Status}.", orderId, newStatus);
                throw new NotFoundException("The specified status does not exist. ");
            }

            var oldStatus = order.Status;
            order.Status = newStatus;
            _orderRepository.Update(order);
            await _orderRepository.SaveChangesAsync();

            _logger.LogInformation("Order {OrderId} status updated from {OldStatus} to {NewStatus}.", orderId, oldStatus, newStatus);

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
                _logger.LogWarning("User {UserId} attempted to cancel Order {OrderId} which was not found.", userId, orderId);
                throw new NotFoundException("Order not found.");
            }

            if (order.Status == OrderStatus.cancelled)
            {
                _logger.LogWarning("User {UserId} attempted to cancel Order {OrderId} but it is already cancelled.", userId, orderId);
                throw new BadRequestException($"Order cannot be cancelled because it is already {order.Status}.");
            }

            var oldStatus = order.Status;
            order.Status = OrderStatus.cancelled;

            var redeemedTransaction = await _loyaltyTransactionRepository.GetTransactionByOrderIdAndTypeAsync(orderId, TransactionType.Redeemed);
            if (redeemedTransaction != null)
            {
                int pointsToRefund = Math.Abs(redeemedTransaction.Points);
                buyer.LoyaltyPoints = (buyer.LoyaltyPoints ?? 0) + pointsToRefund;
                _buyerRepository.Update(buyer);

                var refundTransaction = new LoyaltyTransaction
                {
                    CreatedAt = DateTime.UtcNow,
                    BuyerId = buyer.BuyerId,
                    OrderId = order.OrderId,
                    TransactionType = TransactionType.Refunded,
                    Points = pointsToRefund, 
                };
                await _loyaltyTransactionRepository.AddAsync(refundTransaction);
            }

            if (oldStatus == OrderStatus.successful)
            {
                var earnedTransaction = await _loyaltyTransactionRepository.GetTransactionByOrderIdAndTypeAsync(orderId, TransactionType.Earned);
                if (earnedTransaction != null)
                {
                    buyer.LoyaltyPoints = Math.Max(0, (buyer.LoyaltyPoints ?? 0) - earnedTransaction.Points);
                    _buyerRepository.Update(buyer);

                    var revokeTransaction = new LoyaltyTransaction
                    {
                        CreatedAt = DateTime.UtcNow,
                        BuyerId = buyer.BuyerId,
                        OrderId = order.OrderId,
                        TransactionType = TransactionType.Revoked,
                        Points = -earnedTransaction.Points, 
                    };
                    await _loyaltyTransactionRepository.AddAsync(revokeTransaction);
                }
            }

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

            _logger.LogInformation("Order {OrderId} was successfully cancelled by User {UserId}. Stock, coupons, and loyalty points have been updated.", orderId, userId);

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
                _logger.LogWarning("Payment Callback received for non-existent Order {OrderId}.", orderId);
                throw new NotFoundException("Order not found.");
            }
            if (order.Status == OrderStatus.cancelled)
            {
                _logger.LogWarning("Payment Callback received for Order {OrderId} but order is already cancelled.", orderId);
                throw new BadRequestException("This order is already cancelled");
            }
            if (order.Status == OrderStatus.successful)
            {
                _logger.LogWarning("Payment Callback received for Order {OrderId} but it has already been paid.", orderId);
                throw new BadRequestException("This order has already been payed");
            }

            var buyer = await _buyerRepository.GetByIdAsync(order.BuyerId);
            if (buyer == null)
            {
                throw new NotFoundException("Buyer profile not found.");
            }

            if (isSuccess)
            {
                order.Status = OrderStatus.successful;
                _logger.LogInformation("Payment Callback: Payment SUCCESSFUL for Order {OrderId}. Status updated to successful.", orderId);

                var finalTotal = order.TotalAmount;
                int loyaltyPointsEarned = (int)finalTotal / 1000;

                if (loyaltyPointsEarned > 0)
                {
                    buyer.LoyaltyPoints = (buyer.LoyaltyPoints ?? 0) + loyaltyPointsEarned;
                    _buyerRepository.Update(buyer);

                    var loyaltyTransaction = new LoyaltyTransaction
                    {
                        CreatedAt = DateTime.UtcNow,
                        BuyerId = buyer.BuyerId,
                        OrderId = order.OrderId,
                        TransactionType = TransactionType.Earned,
                        Points = loyaltyPointsEarned,
                    };
                    await _loyaltyTransactionRepository.AddAsync(loyaltyTransaction);
                }
            }
            else
            {
                order.Status = OrderStatus.cancelled;
                _logger.LogInformation("Payment Callback: Payment FAILED for Order {OrderId}. Order cancelled, restoring stock and refunding points.", orderId);

                var redeemedTransaction = await _loyaltyTransactionRepository.GetTransactionByOrderIdAndTypeAsync(orderId, TransactionType.Redeemed);
                if (redeemedTransaction != null)
                {
                    int pointsToRefund = Math.Abs(redeemedTransaction.Points);
                    buyer.LoyaltyPoints = (buyer.LoyaltyPoints ?? 0) + pointsToRefund;
                    _buyerRepository.Update(buyer);

                    var refundTransaction = new LoyaltyTransaction
                    {
                        CreatedAt = DateTime.UtcNow,
                        BuyerId = buyer.BuyerId,
                        OrderId = order.OrderId,
                        TransactionType = TransactionType.Refunded,
                        Points = pointsToRefund,
                    };
                    await _loyaltyTransactionRepository.AddAsync(refundTransaction);
                }

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

        private decimal UsePoints(decimal finaltotal, int pointsToUse)
        {
            int pointsdiscount = pointsToUse / 100;
            finaltotal = finaltotal - pointsdiscount;
            return finaltotal;
        }
    }
}