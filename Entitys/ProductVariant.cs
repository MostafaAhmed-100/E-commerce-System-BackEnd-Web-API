using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Entitys
{
    public class ProductVariant
    {
        public int ProductVariantId { get; set; }

        public  int ProductId { get; set; }
        public required string SKU { get; set; } // Stock Keeping Unit  ParCode
        public required decimal Price { get; set; }
        public int ReservedQuantity { get; set; } = 0;
        public required int QuantityInStock { get; set; }
        public decimal Discount { get; set; } = 0;
        public required string Size { get; set; }
        public required string Color { get; set; }
        public  Product Product { get; set; }
        public Decimal AverageRating { get; set; }
        public int TotalReviews { get; set; }


    }
}
