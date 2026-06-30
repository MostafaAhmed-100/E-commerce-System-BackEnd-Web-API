using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication1.Entitys;

namespace WebApplication1.Data.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(C => C.CategoryId);

            builder.HasIndex(C => C.CategoryName)
               .IsUnique();
            builder.Property(C => C.CategoryName)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasOne(C => C.ParentCategory)
                .WithMany(C => C.SubCategories)
                .OnDelete(DeleteBehavior.NoAction)
                .HasForeignKey(C => C.ParentCategoryId);
            
            
        }
    }
}
