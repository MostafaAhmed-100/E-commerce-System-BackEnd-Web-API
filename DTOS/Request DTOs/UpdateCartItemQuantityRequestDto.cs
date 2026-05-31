using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTOS.Request_DTOs
{
    public class UpdateCartItemQuantityRequestDto
    {
        [Required, Range(1, 100)]
        public int Quantity { get; set; }
    }
}
