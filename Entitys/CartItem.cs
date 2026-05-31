using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Entitys
{
    public class CartItem
    {
        [Key, Required]
        public required int CartItemId { get; set; }
        [Required]
        public required int CartId { get; set; }

        [Required]
        public required int Quantity { get; set; }

        [Required]
        public required int ProductVariantId { get; set; }
        [Required, ForeignKey("CartId")]
        public Cart Cart { get; set; }
        [Required, ForeignKey("ProductVariantId")]
        public ProductVariant ProductVariant { get; set; }


    }
}
