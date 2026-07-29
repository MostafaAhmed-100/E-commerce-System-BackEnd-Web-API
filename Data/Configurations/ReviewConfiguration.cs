using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication1.Entitys;

namespace WebApplication1.Data.Configurations
{
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {

            builder.ToTable("Reviews");
            builder.HasKey(r => r.Id);


            builder.HasIndex(r => new { r.BuyerId, r.ProductVariantId })
                   .IsUnique();


            builder.ToTable(t => t.HasCheckConstraint("CK_Review_Rating", "[Rating] >= 1 AND [Rating] <= 5"));


            builder.Property(r => r.Rating)
                   .IsRequired();

            builder.Property(r => r.Comment)
                   .HasMaxLength(1000)
                   .IsRequired(false);

            builder.Property(r => r.CreatedAt)
                   .IsRequired()
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(r => r.Buyer)
                   .WithMany()
                   .HasForeignKey(r => r.BuyerId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.ProductVariant)
                   .WithMany()
                   .HasForeignKey(r => r.ProductVariantId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
