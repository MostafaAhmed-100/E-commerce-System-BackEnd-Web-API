using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTOS.Response_DTOs
{
    public class ProductVariantResponseDto
    {
        public int VariantId { get; set; }
        public required string SKU { get; set; }
        public required decimal Price { get; set; }
        public required bool IsAvailable { get; set; }
        public required string Color { get; set; }
        public required string Size { get; set; }
    }
}
