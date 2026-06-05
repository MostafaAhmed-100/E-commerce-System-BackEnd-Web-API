using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Entitys
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        [Required]
        public required int AddressId { get; set; }
        [Required]
        public  required int BuyerId { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [Required]
        public bool IsDeleted { get; set; } = false;
        public int? CouponId { get; set; } = null;

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public decimal? DiscountAmount { get; set; } = null;
        [Required]
        public required decimal TotalAmount { get; set; }
        [Required]
        public required string Status { get; set; } = "Pending";

        [ForeignKey(nameof(BuyerId))]
        public Buyer Buyer { get; set; } = null!;
        [ForeignKey(nameof(AddressId))]
        public Address? Address { get; set; } = null;
        public Coupon? Coupon { get; set; } = null;

    }
}
