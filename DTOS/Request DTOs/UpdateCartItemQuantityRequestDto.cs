using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTOS.Request_DTOs
{
    public class UpdateCartItemQuantityRequestDto
    {
        public required int Quantity { get; set; }
    }
}
