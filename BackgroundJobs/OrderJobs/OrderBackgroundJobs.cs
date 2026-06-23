using WebApplication1.Constants;
using WebApplication1.Data;
using WebApplication1.Entitys;
using WebApplication1.Repository.SpecificRepository.CouponRepository;
using WebApplication1.Repository.SpecificRepository.OrderRepository;

namespace WebApplication1.BackgroundJobs.OrderJobs
{
    public class OrderBackgroundJobs : IOrderBackgroundJobs 
    {
        private readonly ICouponRepository _couponRepository;
        private readonly IOrderRepository _orderRepository;

        public OrderBackgroundJobs(ICouponRepository couponRepository ,IOrderRepository orderRepository) 
        {
            _couponRepository = couponRepository ;
            _orderRepository = orderRepository ;
        }
        
        public async Task CheckAndCancelUnpaidOrderAsyn(int OrderId)
        {
            var Order = await _orderRepository.GetByIdAsync(OrderId);
            var OrderWithDetails = await _orderRepository.GetOrderWithDetailsAsync(OrderId , Order.BuyerId);
            var Status = Order.Status;
            if (Status == OrderStatus.Pending)
            {
                Order.Status = OrderStatus.cancelled;
                foreach (var item in OrderWithDetails.OrderItems)
                {
                    item.ProductVariant.QuantityInStock += item.Quantity;
                }
                _orderRepository.Update(Order);
                if (OrderWithDetails.CouponId != null)
                {
                    var Coupon = await _couponRepository.GetByIdAsync(Order.CouponId.Value);
                    Coupon.UsedCount -= 1;
                    _couponRepository.Update(Coupon);
                }
                await _orderRepository.SaveChangesAsync();
            }
        }
    }
}
