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
using WebApplication1.Repository.UnitOfWork;

namespace WebApplication1.Services.OrderService
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentService _paymentService;
        private readonly ISavedCardService _savedCardService;
        private readonly ILogger<OrderService> _logger;
        private readonly IMapper _mapper;

        public OrderService(
            ILogger<OrderService> logger,
            IUnitOfWork unitOfWork,
            ISavedCardService savedCardService,
            IPaymentService paymentService,
            IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _savedCardService = savedCardService;
            _paymentService = paymentService;
            _mapper = mapper;
        }

        public async Task<ApiResponseDto<OrderResponseDto>> CreateOrderAsync(CreateOrderRequestDto createOrderRequestDto, int buyerId, int userId)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var cart = await _unitOfWork.CartRepository.GetCartWithItemsAsync(buyerId);
                if (cart == null || !cart.Items.Any())
                {
                    _logger.LogWarning("User {UserId} attempted to create an order but cart is empty for Buyer {BuyerId}.", userId, buyerId);
                    throw new BadRequestException("Your cart has no items to order.");
                }

                var buyer = await _unitOfWork.BuyerRepository.GetBuyerByUserId(userId);
                if (buyer == null || buyerId != buyer.BuyerId)
                {
                    _logger.LogWarning("Security Warning: User {UserId} attempted to create an order for unauthorized BuyerId {AttemptedBuyerId}.", userId, buyerId);
                    throw new UnauthorizedException("The provided Buyer ID does not belong to the authenticated user");
                }

                bool isRedeemingPoints = createOrderRequestDto.UseLoyaltyPoints == true && createOrderRequestDto.PointsToRedeem != null;

                if (isRedeemingPoints)
                {
                    var points = await _unitOfWork.LoyaltyTransactionRepository.GetTotalPointsFromLedgerByBuyerIdAsync(buyerId);
                    if (points < createOrderRequestDto.PointsToRedeem)
                    {
                        _logger.LogWarning("The User {UserId} attempted to use points {PointsToRedeem} he doesn't have, he has {Points}", userId, createOrderRequestDto.PointsToRedeem, points);
                        throw new BadRequestException($"You cant redeem {createOrderRequestDto.PointsToRedeem} points, you only have {points}");
                    }
                }

                var address = await _unitOfWork.AddressRepository.GetByIdAsync(createOrderRequestDto.AddressId);
                if (address == null || address.UserId != userId)
                {
                    _logger.LogWarning("User {UserId} provided an invalid or unauthorized AddressId {AddressId}.", userId, createOrderRequestDto.AddressId);
                    throw new NotFoundException("The specified address was not found or does not belong to you.");
                }

                var savedCard = await _savedCardService.GetMyCardsAsync(userId);
                if (!savedCard.Data.Any())
                {
                    _logger.LogWarning("User {UserId} attempted to create an order without any saved cards.", userId);
                    throw new NotFoundException("You Dont Have any Cards ");
                }

                decimal subtotal = 0;
                foreach (var item in cart.Items)
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
                    appliedCoupon = await _unitOfWork.CouponRepository.GetCouponByCodeAsync(createOrderRequestDto.CouponCode);

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
                        _logger.LogWarning("The User {UserId} attempted to redeem points that equals less than 100 EGP", userId);
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
                    OrderItems = cart.Items.Select(ci => new OrderItem
                    {
                        ProductVariantId = ci.ProductVariantId,
                        Quantity = ci.Quantity,
                        Price = ci.ProductVariant.Price,
                        ProductVariant = ci.ProductVariant,
                    }).ToList()
                };

                await _unitOfWork.OrderRepository.AddAsync(order);

                foreach (var item in cart.Items)
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
                    _unitOfWork.CouponRepository.Update(appliedCoupon);
                }

                _unitOfWork.CartRepository.Delete(cart);

                if (isRedeemingPoints)
                {
                    buyer.LoyaltyPoints = (buyer.LoyaltyPoints ?? 0) - (int)createOrderRequestDto.PointsToRedeem;
                    _unitOfWork.BuyerRepository.Update(buyer);

                    var loyaltyTransaction = new LoyaltyTransaction
                    {
                        CreatedAt = DateTime.UtcNow,
                        BuyerId = buyerId,
                        OrderId = order.OrderId,
                        TransactionType = TransactionType.Redeemed,
                        Points = -(int)createOrderRequestDto.PointsToRedeem,
                    };
                    await _unitOfWork.LoyaltyTransactionRepository.AddAsync(loyaltyTransaction);
                }

                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Order {OrderId} successfully created for Buyer {BuyerId} (User {UserId}) with TotalAmount {TotalAmount}.", order.OrderId, buyer.BuyerId, userId, finalTotal);

                BackgroundJob.Schedule<IOrderBackgroundJobs>(
                    job => job.CheckAndCancelUnpaidOrderAsyn(order.OrderId),
                    TimeSpan.FromMinutes(30));

                var paymentRequest = new PaymentRequestDto
                {
                    CustomerEmail = buyer.User.Email,
                    CurrencyCode = createOrderRequestDto.CurrencyCode,
                    OrderId = order.OrderId,
                    CustomerName = buyer.User.UserName,
                    TotalAmount = finalTotal,
                };

                var initializePaymentResponse = await _paymentService.InitializePayment(paymentRequest);
                if (initializePaymentResponse.IsSuccess == false)
                {
                    _logger.LogWarning("Payment initialization failed for Order {OrderId} belonging to User {UserId}.", order.OrderId, userId);
                    throw new BadRequestException("الرابط الخارجي غير صحيح أو السيرفر غير متاح. The payment has been declined. Order Status is Pending.");
                }

                _logger.LogInformation("Payment successfully initialized for Order {OrderId}. Checkout URL generated.", order.OrderId);

                var orderResponse = _mapper.Map<OrderResponseDto>(order);
                orderResponse.CheckoutUrl = initializePaymentResponse.CheckoutUrl;

                return new ApiResponseDto<OrderResponseDto>
                {
                    Message = "Order created successfully.",
                    Data = orderResponse
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error occurred while creating order for User {UserId}", userId);
                throw;
            }
        }

        public async Task<ApiResponseDto<PaginatedResponseDto<OrderResponseDto>>> GetOrdersByBuyerIdAsync(int buyerId, int userId, PaginationRequestDto paginationRequestDto)
        {
            try
            {
                var buyer = await _unitOfWork.BuyerRepository.GetBuyerByUserId(userId);
                if (buyer == null || buyer.BuyerId != buyerId)
                {
                    _logger.LogWarning("Security Warning: User {UserId} attempted to view orders for unauthorized BuyerId {AttemptedBuyerId}.", userId, buyerId);
                    throw new UnauthorizedException("The provided Buyer ID does not belong to the authenticated user.");
                }

                var (orders, totalCount) = await _unitOfWork.OrderRepository.GetOrdersListByBuyerIdAsync(
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving orders for buyer {BuyerId}", buyerId);
                throw;
            }
        }

        public async Task<ApiResponseDto<OrderResponseDto>> GetOrderByIdAsync(int orderId, int userId)
        {
            try
            {
                var buyer = await _unitOfWork.BuyerRepository.GetBuyerByUserId(userId);
                if (buyer == null)
                {
                    throw new BadRequestException("User profile not found.");
                }

                var order = await _unitOfWork.OrderRepository.GetOrderWithDetailsAsync(orderId, buyer.BuyerId);

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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving order {OrderId} for user {UserId}", orderId, userId);
                throw;
            }
        }

        public async Task<ApiResponseDto<string>> UpdateOrderStatusAsync(int orderId, string newStatus)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);

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
                _unitOfWork.OrderRepository.Update(order);
                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Order {OrderId} status updated from {OldStatus} to {NewStatus}.", orderId, oldStatus, newStatus);

                return new ApiResponseDto<string>
                {
                    Message = $"Order status updated to {newStatus} successfully.",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error occurred while updating status for order {OrderId}", orderId);
                throw;
            }
        }

        public async Task<ApiResponseDto<string>> CancelOrderAsync(int orderId, int userId)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var buyer = await _unitOfWork.BuyerRepository.GetBuyerByUserId(userId);
                if (buyer == null)
                {
                    throw new NotFoundException("User profile not found.");
                }

                var order = await _unitOfWork.OrderRepository.GetOrderWithDetailsAsync(orderId, buyer.BuyerId);

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

                var redeemedTransaction = await _unitOfWork.LoyaltyTransactionRepository.GetTransactionByOrderIdAndTypeAsync(orderId, TransactionType.Redeemed);
                if (redeemedTransaction != null)
                {
                    int pointsToRefund = Math.Abs(redeemedTransaction.Points);
                    buyer.LoyaltyPoints = (buyer.LoyaltyPoints ?? 0) + pointsToRefund;
                    _unitOfWork.BuyerRepository.Update(buyer);

                    var refundTransaction = new LoyaltyTransaction
                    {
                        CreatedAt = DateTime.UtcNow,
                        BuyerId = buyer.BuyerId,
                        OrderId = order.OrderId,
                        TransactionType = TransactionType.Refunded,
                        Points = pointsToRefund,
                    };
                    await _unitOfWork.LoyaltyTransactionRepository.AddAsync(refundTransaction);
                }

                if (oldStatus == OrderStatus.successful)
                {
                    var earnedTransaction = await _unitOfWork.LoyaltyTransactionRepository.GetTransactionByOrderIdAndTypeAsync(orderId, TransactionType.Earned);
                    if (earnedTransaction != null)
                    {
                        buyer.LoyaltyPoints = Math.Max(0, (buyer.LoyaltyPoints ?? 0) - earnedTransaction.Points);
                        _unitOfWork.BuyerRepository.Update(buyer);

                        var revokeTransaction = new LoyaltyTransaction
                        {
                            CreatedAt = DateTime.UtcNow,
                            BuyerId = buyer.BuyerId,
                            OrderId = order.OrderId,
                            TransactionType = TransactionType.Revoked,
                            Points = -earnedTransaction.Points,
                        };
                        await _unitOfWork.LoyaltyTransactionRepository.AddAsync(revokeTransaction);
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
                    var coupon = await _unitOfWork.CouponRepository.GetByIdAsync(order.CouponId.Value);
                    if (coupon != null && coupon.UsedCount > 0)
                    {
                        coupon.UsedCount -= 1;
                        _unitOfWork.CouponRepository.Update(coupon);
                    }
                }

                _unitOfWork.OrderRepository.Update(order);
                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Order {OrderId} was successfully cancelled by User {UserId}.", orderId, userId);

                return new ApiResponseDto<string>
                {
                    Message = "Order cancelled successfully and stock has been restored.",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error occurred while cancelling order {OrderId}", orderId);
                throw;
            }
        }

        public async Task<ApiResponseDto<string>> HandlePaymentCallbackAsync(int orderId, bool isSuccess)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var order = await _unitOfWork.OrderRepository.GetOrderWithItemsByIdAsync(orderId);

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

                var buyer = await _unitOfWork.BuyerRepository.GetByIdAsync(order.BuyerId);
                if (buyer == null)
                {
                    throw new NotFoundException("Buyer profile not found.");
                }

                if (isSuccess)
                {
                    order.Status = OrderStatus.successful;
                    _logger.LogInformation("Payment Callback: Payment SUCCESSFUL for Order {OrderId}.", orderId);

                    var finalTotal = order.TotalAmount;
                    int loyaltyPointsEarned = (int)finalTotal / 1000;

                    if (loyaltyPointsEarned > 0)
                    {
                        buyer.LoyaltyPoints = (buyer.LoyaltyPoints ?? 0) + loyaltyPointsEarned;
                        _unitOfWork.BuyerRepository.Update(buyer);

                        var loyaltyTransaction = new LoyaltyTransaction
                        {
                            CreatedAt = DateTime.UtcNow,
                            BuyerId = buyer.BuyerId,
                            OrderId = order.OrderId,
                            TransactionType = TransactionType.Earned,
                            Points = loyaltyPointsEarned,
                        };
                        await _unitOfWork.LoyaltyTransactionRepository.AddAsync(loyaltyTransaction);
                    }
                }
                else
                {
                    order.Status = OrderStatus.cancelled;
                    _logger.LogInformation("Payment Callback: Payment FAILED for Order {OrderId}.", orderId);

                    var redeemedTransaction = await _unitOfWork.LoyaltyTransactionRepository.GetTransactionByOrderIdAndTypeAsync(orderId, TransactionType.Redeemed);
                    if (redeemedTransaction != null)
                    {
                        int pointsToRefund = Math.Abs(redeemedTransaction.Points);
                        buyer.LoyaltyPoints = (buyer.LoyaltyPoints ?? 0) + pointsToRefund;
                        _unitOfWork.BuyerRepository.Update(buyer);

                        var refundTransaction = new LoyaltyTransaction
                        {
                            CreatedAt = DateTime.UtcNow,
                            BuyerId = buyer.BuyerId,
                            OrderId = order.OrderId,
                            TransactionType = TransactionType.Refunded,
                            Points = pointsToRefund,
                        };
                        await _unitOfWork.LoyaltyTransactionRepository.AddAsync(refundTransaction);
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
                        var coupon = await _unitOfWork.CouponRepository.GetByIdAsync(order.CouponId.Value);
                        if (coupon != null && coupon.UsedCount > 0)
                        {
                            coupon.UsedCount -= 1;
                            _unitOfWork.CouponRepository.Update(coupon);
                        }
                    }
                }

                _unitOfWork.OrderRepository.Update(order);
                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();

                return new ApiResponseDto<string>
                {
                    Message = isSuccess ? "Payment successful. Order is processing." : "Payment failed. Order cancelled and stock restored.",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error occurred while handling payment callback for order {OrderId}", orderId);
                throw;
            }
        }

        private decimal UsePoints(decimal finaltotal, int pointsToUse)
        {
            int pointsdiscount = pointsToUse / 100;
            finaltotal = finaltotal - pointsdiscount;
            return finaltotal;
        }
    }
}