using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication1.Entitys;

namespace WebApplication1.Data.Configurations
{
    public class SellerConfiguration : IEntityTypeConfiguration<Seller>
    {
        public void Configure(EntityTypeBuilder<Seller> builder)
        {
            builder.HasKey(S => S.SellerId);

            builder.HasIndex(S => S.BankAccountNumber)
                .IsUnique();
            builder.Property(S => S.BankAccountNumber)
                .HasMaxLength(90)
                .IsRequired();

            builder.HasIndex(S => S.TaxNumber)
                .IsUnique();
            builder.Property(S => S.TaxNumber)
                .HasMaxLength(90)
                .IsRequired();

            builder.HasIndex(S => S.PhoneNumber)
                .IsUnique();
            builder.Property(S => S.PhoneNumber)
                .IsRequired();

            builder.HasIndex(S => S.StoreName)
                .IsUnique();
            builder.Property(S => S.StoreName)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(S => S.BankName)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(S => S.NationalId)
                .IsUnique();
            builder.Property(S => S.NationalId)
                .IsRequired()
                .HasMaxLength(14);

            builder.HasQueryFilter(S => S.IsDeleted == false);
        }
    }
}
