using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTOS.Request_DTOs
{
    public class CreateOrderRequestDto
    {
        public required int AddressId { get; set; }
        public int? SavedCardId { get; set; }
        public string? CouponCode { get; set; }
        
        public string CurrencyCode { get; set; }
    }
}
