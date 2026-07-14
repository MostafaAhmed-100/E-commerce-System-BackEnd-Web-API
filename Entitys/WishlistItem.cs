namespace WebApplication1.Entitys
{
    public class WishlistItem
    {
        public int WishlistItemId { get; set; }

        public int WishlistId { get; set; }

        public int productVariantId { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        public Wishlist wishlist { get; set; }

        public ProductVariant productVariant{ get; set; }
    }
}
