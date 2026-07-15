using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication1.Entitys;

namespace WebApplication1.Data.Configurations
{
    public class LoyaltyTransactionConfiguration : IEntityTypeConfiguration<LoyaltyTransaction>
    {
        public void Configure(EntityTypeBuilder<LoyaltyTransaction> builder)
        {
            builder.ToTable("LoyaltyTransactions");

            builder.HasKey(lt => lt.LoyaltyTransactionId);

            builder.Property(lt => lt.Points)
                   .IsRequired();

            builder.Property(lt => lt.TransactionType)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.Property(lt => lt.CreatedAt)
                   .IsRequired()
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(lt => lt.Buyer)
                   .WithMany(lt => lt.loyaltyTransactions)
                   .HasForeignKey(lt => lt.BuyerId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(lt => lt.Order)
                   .WithMany(lt => lt.loyaltyTransactions)
                   .HasForeignKey(lt => lt.OrderId)
                   .IsRequired(false)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}