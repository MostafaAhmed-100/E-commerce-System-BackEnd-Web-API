namespace WebApplication1.Entitys
{
    public class SavedCard
    {
        public int CardId { get; set; }

        public required int UserId { get; set; }

        public required string CardBrand { get; set; }

        public required string MaskedNumber { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public required string CardToken { get; set; }

        public bool IsActive { get; set; } = true;

        public required User User { get; set; }
    }
}