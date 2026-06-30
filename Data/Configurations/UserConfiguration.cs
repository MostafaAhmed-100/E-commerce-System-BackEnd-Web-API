using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication1.Entitys;

namespace WebApplication1.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasOne(U => U.Seller)
                .WithOne(U => U.User)
                .HasForeignKey<Seller>(U => U.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(U => U.Buyer)
                .WithOne(U => U.User)
                .HasForeignKey<Buyer>(U => U.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(U => U.Addresses)
                .WithOne(U => U.User)
                .HasForeignKey(U => U.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
