namespace WebApplication1.Entitys
{
    public class Review
    {
        public int Id { get; set; }
        public int BuyerId { get; set; }
        public int ProductVariantId { get; set; }
        public int Rating { get; set; }
        public string? Comment {  get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public Buyer Buyer { get; set; }
        public ProductVariant ProductVariant{ get; set; }
    }
}
