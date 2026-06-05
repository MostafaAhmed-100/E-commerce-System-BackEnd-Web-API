using System.ComponentModel.DataAnnotations;
using WebApplication1.Entitys;

namespace WebApplication1.DTOS.Request_DTOs
{
    public class UpdateCouponRequestDto
    {
        [Required]
        public required DiscountType DiscountType { get; set; }

        [Required]
        public required decimal DiscountValue { get; set; }

        [Required]
        public required DateTime StartDate { get; set; }

        [Required]
        public required DateTime EndDate { get; set; }

        public int? UsageLimit { get; set; }
    }
}