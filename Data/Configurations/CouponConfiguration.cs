using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication1.Entitys;

namespace WebApplication1.Data.Configurations
{
    public class CouponConfiguration : IEntityTypeConfiguration<Coupon>
    {
        public void Configure(EntityTypeBuilder<Coupon> builder)
        {
            builder.HasKey(c => c.CouponId);

            builder.HasIndex(c => c.CouponCode)
                   .IsUnique();
            builder.Property(c => c.CouponCode)
                   .IsRequired()
                   .HasMaxLength(10);

            builder.Property(c => c.DiscountValue)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(c => c.UsedCount)
                   .HasDefaultValue(0);

            builder.HasCheckConstraint("CK_Coupon_Dates", "[EndDate] >= [StartDate]");
            builder.HasCheckConstraint("CK_Coupon_UsedCount", "[UsedCount] >= 0");

            builder.HasOne(c => c.Seller)
                   .WithMany()
                   .HasForeignKey(c => c.SellerId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}