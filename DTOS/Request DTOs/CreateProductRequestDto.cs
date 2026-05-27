using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTOS.Request_DTOs
{
    public class CreateProductRequestDto
    {
        [Required, MaxLength(100)]
        public required string ProductName { get; set; }
        [Required, MaxLength(250)]
        public required string ProductDescription { get; set; }
        [Required, MaxLength(250)]
        public required string ImagePath { get; set; }
        [Required]
        public required int CategoryId { get; set; }
        [Required, MinLength(1)]
        public required List<CreateProductVariantRequestDto> Variants { get; set; }
    }
}
