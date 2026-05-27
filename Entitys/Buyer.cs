using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Entitys
{
    public class Buyer
    {
        [Key]
        public required int BuyerId { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required, MaxLength(10000)]
        public int LoyaltyPoints { get; set; } = 0;
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [Required]
        public bool IsDeleted { get; set; } = false;
        [Required]
        public required string PaymentGatewayCustomerId { get; set; }
        [ForeignKey("UserId"), Required]
        public required User User { get; set; }
        public ICollection<Order> Orders { get; set; } = new List<Order>();

    }
}
