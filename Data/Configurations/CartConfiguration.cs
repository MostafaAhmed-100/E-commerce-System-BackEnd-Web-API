using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication1.Entitys;

namespace WebApplication1.Data.Configurations
{
    public class CartConfiguration : IEntityTypeConfiguration<Cart>
    {
        public void Configure(EntityTypeBuilder<Cart> builder)
        {
            builder.HasKey(c => c.CartId);

            builder.HasOne(c => c.Buyer)
                   .WithOne()
                   .HasForeignKey<Cart>(c => c.BuyerId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}