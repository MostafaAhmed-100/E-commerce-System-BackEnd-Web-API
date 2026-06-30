using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication1.Entitys;

namespace WebApplication1.Data.Configurations
{
    public class BuyerConfiguration : IEntityTypeConfiguration<Buyer>
    {
        public void Configure(EntityTypeBuilder<Buyer> builder)
        {
            builder.HasKey(B => B.BuyerId);

            builder.HasIndex(B => B.PaymentGatewayCustomerId)
                .IsUnique();
            builder.Property(B => B.PaymentGatewayCustomerId)
                .HasMaxLength(250);

            builder.Property(B => B.LoyaltyPoints)
                .HasDefaultValue(0);
            builder.HasCheckConstraint("CK_Buyer_LoyaltyPoints", "[LoyaltyPoints] >= 0");

            builder.Property(B => B.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(B => B.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasQueryFilter(S => S.IsDeleted == false);
        }
    }
}
