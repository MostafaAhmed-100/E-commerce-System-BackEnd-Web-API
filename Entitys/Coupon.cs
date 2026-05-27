using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Entitys
{
    public class Coupon
    {
        [Key]
        public int CouponId { get; set; }

        [Required, MaxLength(10), MinLength(3)]
        public required string CouponCode { get; set; }

        [Required]
        public DiscountType DiscountType { get; set; }
        [Required]
        public required decimal DiscountValue { get; set; }

        [Required]
        public required DateTime StartDate { get; set; }
        [Required]
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