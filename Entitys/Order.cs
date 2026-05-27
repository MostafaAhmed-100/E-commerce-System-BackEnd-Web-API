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
        [Required]
        public string? CouponId { get; set; } = null;
        [Required]
        public decimal? DiscountAmount { get; set; } = null;
        [Required]
        public required int CartId { get; set; }
        [ForeignKey(nameof(CartId))]
        public Cart Cart { get; set; } = null!;
        [ForeignKey(nameof(BuyerId))]
        public Buyer Buyer { get; set; } = null!;
        [ForeignKey(nameof(AddressId))]
        public Address? Address { get; set; } = null;
        public Coupon? Coupon { get; set; } = null;

    }
}
