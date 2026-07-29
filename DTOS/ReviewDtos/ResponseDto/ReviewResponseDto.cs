namespace WebApplication1.DTOS.ReviewDtos.ResponseDto
{
    public class ReviewResponseDto
    {
        public int ReviewId { get; set; }
        public int BuyerId { get; set; }
        public string BuyerName { get; set; } = string.Empty;
        public int ProductVariantId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
