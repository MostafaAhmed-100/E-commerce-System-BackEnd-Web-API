using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication1.Entitys;

namespace WebApplication1.Data.Configurations
{
    public class WishlistItemConfiguration : IEntityTypeConfiguration<WishlistItem>
    {
        public void Configure(EntityTypeBuilder<WishlistItem> builder)
        {
            builder.HasKey(wi => wi.WishlistItemId);

            builder.HasOne(wi => wi.wishlist)
                .WithMany(w => w.Items)
                .HasForeignKey(wi => wi.WishlistId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(wi => wi.product)
                .WithMany() 
                .HasForeignKey(wi => wi.ProductId)
                .OnDelete(DeleteBehavior.Restrict); 
        }
    }
}