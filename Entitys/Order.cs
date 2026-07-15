using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Constants;

namespace WebApplication1.Entitys
{
    public class Order
    {
        public int OrderId { get; set; }
        public required int AddressId { get; set; }
        public  required int BuyerId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsDeleted { get; set; } = false;
        public int? CouponId { get; set; } = null;

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public decimal? DiscountAmount { get; set; } = null;
        public required decimal TotalAmount { get; set; }
        public required string Status { get; set; } = OrderStatus.Pending;
        public Buyer Buyer { get; set; } = null!;
        public Address? Address { get; set; } = null;
        public Coupon? Coupon { get; set; } = null;
        public ICollection<LoyaltyTransaction> loyaltyTransactions { get; set; }

    }
}
