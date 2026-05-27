using WebApplication1.Entitys;

namespace WebApplication1.DTOS.Request_DTOs
{
    public class CreateCouponRequestDto
    {
        public required string CouponCode { get; set; }

        public required DiscountType DiscountType { get; set; }

        public required decimal DiscountValue { get; set; }
        public required DateTime ExpiryDate { get; set; }
        public required DateTime StartDate { get; set; }
        public required int? UsageLimit { get; set; }
        public required int? SellerId { get; set; }
    }
}
