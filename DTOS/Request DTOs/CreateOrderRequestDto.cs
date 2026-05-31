using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTOS.Request_DTOs
{
    public class CreateOrderRequestDto
    {
        public required int AddressId { get; set; }
            [MaxLength(50)]
        public string? CouponCode { get; set; }
    }
}
