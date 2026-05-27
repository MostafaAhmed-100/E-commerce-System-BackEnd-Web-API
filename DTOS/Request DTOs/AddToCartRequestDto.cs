using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTOS.Request_DTOs
{
    public class AddToCartRequestDto
    {
        public required int ProductVariantId { get; set; }
        [Required,Range(1,100,ErrorMessage = "Quantity must be between 1 and 100")]
        public required int Quantity { get; set; }
    }
}
