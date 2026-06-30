using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication1.Constants;
using WebApplication1.Entitys;

namespace WebApplication1.Data.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.OrderId);

            builder.Property(o => o.TotalAmount)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(o => o.DiscountAmount)
                   .HasColumnType("decimal(18,2)");

            builder.HasCheckConstraint("CK_Order_TotalAmount", "[TotalAmount] >= 0");

            builder.Property(o => o.Status)
                   .IsRequired()
                   .HasMaxLength(50)
                   .HasDefaultValue(OrderStatus.Pending);

            builder.Property(o => o.CreatedAt)
                   .IsRequired()
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(o => o.IsDeleted)
                   .IsRequired()
                   .HasDefaultValue(false);

            builder.HasQueryFilter(o => o.IsDeleted == false);

            builder.HasOne(o => o.Buyer)
                   .WithMany(b => b.Orders)
                   .HasForeignKey(o => o.BuyerId)
                   .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(o => o.Address)
                   .WithMany()
                   .HasForeignKey(o => o.AddressId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}