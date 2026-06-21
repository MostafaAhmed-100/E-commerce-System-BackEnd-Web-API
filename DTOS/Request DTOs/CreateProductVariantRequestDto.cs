using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTOS.Request_DTOs
{
    public class CreateProductVariantRequestDto
    {
        public required string SKU { get; set; }
        public required decimal Price { get; set; }
        public required int QuantityInStock { get; set; }
        public  string? Color { get; set; }
        public  string? Size { get; set; }
    }
}
