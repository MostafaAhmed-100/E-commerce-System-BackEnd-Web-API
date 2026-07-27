using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTOS.Request_DTOs
{
    public class CreateOrderRequestDto
    {
        public int AddressId { get; set; }
        public int? SavedCardId { get; set; }
        public string? CouponCode { get; set; }
        
        public string CurrencyCode { get; set; }

        public bool UseLoyaltyPoints { get; set; } = false;

        public int? PointsToRedeem { get; set; }
    }
}
