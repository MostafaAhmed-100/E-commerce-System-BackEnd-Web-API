namespace WebApplication1.DTOS.ReviewRequestDto
{
    public class CreateReviewRequestDto
    {
        public int ProductVariantId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}
