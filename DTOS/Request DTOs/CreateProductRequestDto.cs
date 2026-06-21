using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTOS.Request_DTOs
{
    public class CreateProductRequestDto
    {
        public required string ProductName { get; set; }
        public required string ProductDescription { get; set; }
        public required int CategoryId { get; set; }
        public required List<CreateProductVariantRequestDto> Variants { get; set; }
    }
}
