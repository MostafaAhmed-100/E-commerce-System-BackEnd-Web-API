using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Entitys
{
    public class ProductVariant
    {
        [Key]
        public int ProductVariantId { get; set; }

        public  int ProductId { get; set; }
        [Required]
        public required string SKU { get; set; } // Stock Keeping Unit  ParCode
        [Required]
        public required decimal Price { get; set; }
        public int ReservedQuantity { get; set; } = 0;
        [Required]
        public required int QuantityInStock { get; set; }
        [Required]
        public decimal Discount { get; set; } = 0;
        [Required]
        public required string Size { get; set; }
        [Required]
        public required string Color { get; set; }
        [ForeignKey("ProductId")]
        public  Product Product { get; set; }

    }
}
