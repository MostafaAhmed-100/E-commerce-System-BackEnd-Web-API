using WebApplication1.DTOS.Shared;
using WebApplication1.Entitys;

namespace WebApplication1.DTOS.Response_DTOs
{
    public class BuyerProfileResponseDto
    {
        public required string BuyerName { get; set; }
        public required string BuyerEmail { get; set; }
        public required string BuyerPhoneNumber { get; set; }
        public ICollection<Addressdto> Addresses { get; set; }

        public int? LoyaltyPoints { get; set; }

    }
}
