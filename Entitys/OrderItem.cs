using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Entitys
{
    public class OrderItem
    {
        [Key]
        public int OrderItemId { get; set; }
        [Required]
        public int OrderId { get; set; }
        [Required]
        public int ProductVariantId { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public decimal Price { get; set; }

        [ForeignKey("OrderId")]
        public Order Order { get; set; }
        [ForeignKey("ProductVariantId")]
        public required ProductVariant ProductVariant { get; set; }
    }
}
