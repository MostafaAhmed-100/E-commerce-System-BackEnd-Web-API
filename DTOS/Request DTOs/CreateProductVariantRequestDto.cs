using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTOS.Request_DTOs
{
    public class CreateProductVariantRequestDto
    {
        [Required]
        public required string SKU { get; set; }
        [Required, Range(0, 1000000, ErrorMessage = "Price must be a positive number")]
        public required decimal Price { get; set; }
        [Required, Range(0, 1000000, ErrorMessage = "Quantity in stock must be a positive number")]
        public required int QuantityInStock { get; set; }
        [Required]
        public required string Color { get; set; }
        [Required]
        public required string Size { get; set; }
    }
}
