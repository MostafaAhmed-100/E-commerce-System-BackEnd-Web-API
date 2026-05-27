using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTOS.Request_DTOs
{
    public class CreateOrderRequestDto
    {
        [Required]
        public required int CartId { get; set; }
        [Required, MaxLength(20)]
        public required string ShippingZipCode { get; set; }
        [Required]
        public required string ShippingStreet { get; set; }
        [Required, MaxLength(100)]
        public required string ShippingState { get; set; }
        [Required, MaxLength(100)]
        public required string ShippingCity { get; set; }
        [MaxLength(50)]
        public string? CouponCode { get; set; }
    }
}
