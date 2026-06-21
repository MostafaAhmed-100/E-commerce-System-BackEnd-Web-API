using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTOS.Request_DTOs
{
    public class CreateOrderRequestDto
    {
        public required int AddressId { get; set; }

        public string? CouponCode { get; set; }
    }
}
