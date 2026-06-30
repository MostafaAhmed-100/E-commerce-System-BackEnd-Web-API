using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Entitys
{
    public class Coupon
    {
        public int CouponId { get; set; }

        public required string CouponCode { get; set; }

        public DiscountType DiscountType { get; set; }
        public required decimal DiscountValue { get; set; }
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
        public required int? UsageLimit { get; set; }
        public int? UsedCount { get; set; }
        public int? SellerId { get; set; } = null;
        public Seller? Seller { get; set; } = null;
    }

    public enum DiscountType
    {
        Percentage,
        FixedAmount
    }
}