using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication1.Entities;

namespace WebApplication1.Data.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(p => p.ProductId);
            
            builder.HasIndex(p => p.ProductId);

            builder.Property(p => p.ProductName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(p => p.ProductDescription)
                   .IsRequired()
                   .HasMaxLength(1000);

            builder.Property(p => p.ImagePath)
                   .IsRequired(false);

            builder.Property(p => p.CreatedAt)
                   .IsRequired();

            builder.Property(p => p.IsDeleted)
                   .IsRequired();

            builder.HasQueryFilter(p => !p.IsDeleted);

            builder.HasOne(p => p.Category)
                   .WithMany(c => c.Products)
                   .HasForeignKey(p => p.CategoryId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Seller)
                   .WithMany(s => s.products)
                   .HasForeignKey(p => p.SellerId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}