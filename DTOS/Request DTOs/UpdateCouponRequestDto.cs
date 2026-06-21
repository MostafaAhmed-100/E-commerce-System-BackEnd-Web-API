using System.ComponentModel.DataAnnotations;
using WebApplication1.Entitys;

namespace WebApplication1.DTOS.Request_DTOs
{
    public class UpdateCouponRequestDto
    {
        public required DiscountType DiscountType { get; set; }
        public required decimal DiscountValue { get; set; }
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
        public int? UsageLimit { get; set; }
    }
}