using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication1.Entitys;

namespace WebApplication1.Data.Configurations
{
    public class WishlistConfiguration : IEntityTypeConfiguration<Wishlist>
    {
        public void Configure(EntityTypeBuilder<Wishlist> builder)
        {
            builder.HasKey(w => w.WishlistId);

            builder.Property(w => w.WishlistName)
                .IsRequired()
                .HasMaxLength(100);
            
            builder.HasOne(w => w.Buyer)
                .WithMany() 
                .HasForeignKey(w => w.BuyerId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}