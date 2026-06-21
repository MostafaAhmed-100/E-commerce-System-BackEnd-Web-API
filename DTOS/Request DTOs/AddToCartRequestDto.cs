using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTOS.Request_DTOs
{
    public class AddToCartRequestDto
    {
        public required int ProductVariantId { get; set; }
        public required int Quantity { get; set; }
    }
}
