namespace WebApplication1.DTOS.Response_DTOs
{
    public class WishlistItemResponseDto
    {
        public int ItemId { get; set; }

        public decimal ProductPrice { get; set; }

        public string ProductName { get; set; }

        public DateTime AddedAt { get; set; }
    }
}
