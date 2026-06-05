using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTOS.Request_DTOs
{
    public class UpdateProductVariantRequestDto
    {
        public int? VariantId { get; set; } 

        [Required]
        public required string SKU { get; set; }

        [Required, Range(0, 1000000, ErrorMessage = "Price must be between 0 and 1000000")]
        public required decimal Price { get; set; }

        [Required, Range(0, 1000000, ErrorMessage = "Quantity in stock must be a positive number")]
        public required int QuantityInStock { get; set; }

        public string? Color { get; set; }
        public string? Size { get; set; }
    }
}
