namespace WebApplication1.Entitys
{
    public class Wishlist
    {
        public int WishlistId { get; set; }

        public required string WishlistName { get; set; }

        public required int BuyerId { get; set; }

        public required DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public required Buyer Buyer { get; set; }

        public ICollection<WishlistItem> Items { get; set; } = new List<WishlistItem>();
    }
}
