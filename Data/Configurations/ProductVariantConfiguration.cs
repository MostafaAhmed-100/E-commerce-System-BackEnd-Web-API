using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication1.Entitys;

namespace WebApplication1.Data.Configurations
{
    public class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
    {
        public void Configure(EntityTypeBuilder<ProductVariant> builder)
        {
            builder.HasKey(PV => PV.ProductVariantId);

            builder.HasIndex(p => p.SKU)
                .IsUnique();
            builder.Property(p => p.SKU)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(p => p.Price)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(p => p.Discount)
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0)
                .IsRequired();

            builder.Property(p => p.ReservedQuantity)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(p => p.QuantityInStock)
                .IsRequired()
                .HasDefaultValue(0);
            builder.HasCheckConstraint("CK_Variant_Quantities", "[QuantityInStock] >= 0 AND [ReservedQuantity] >= 0");
            
            builder.Property(p => p.Color)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(p => p.Size)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasOne(p => p.Product)
                .WithMany(p => p.ProductVariants)
                .HasForeignKey(p => p.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(pv => pv.AverageRating)
                .HasColumnType("decimal(3, 2)")
                .HasDefaultValue(0m);

            builder.Property(pv => pv.TotalReviews)
                   .HasDefaultValue(0);
        }
    }
}
