using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication1.Entitys;

namespace WebApplication1.Data.Configurations
{
    public class AddressConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.HasKey(a => a.AddressId);

            builder.Property(a => a.City)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(a => a.HomeAddress)
                   .IsRequired()
                   .HasMaxLength(250);

            builder.Property(a => a.State)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(a => a.Zip_Code)
                   .IsRequired()
                   .HasMaxLength(20);
        }
    }
}