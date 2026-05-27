using WebApplication1.Entitys;

namespace WebApplication1.DTOS.Response_DTOs
{
    public class CouponResponseDto
    {
        public required int CouponId { get; set; }

        public required string CouponCode { get; set; }

        public required DiscountType DiscountType { get;set; }

        public required decimal DiscountValue { get; set; }

        public required bool IsActive { get; set; }
    }
}
