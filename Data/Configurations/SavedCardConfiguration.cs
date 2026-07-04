using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication1.Entitys;

namespace WebApplication1.Data.Configurations
{
    public class SavedCardConfiguration : IEntityTypeConfiguration<SavedCard>
    {
        public void Configure(EntityTypeBuilder<SavedCard> builder)
        {
            builder.HasKey(S => S.CardId);

            builder.HasOne(S => S.User)
                .WithMany()
                .HasForeignKey(S => S.UserId);

            builder.Property(S => S.CardBrand)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(S => S.MaskedNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(S => S.CardToken)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(S => S.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

        }
    }
}
