using Domain.Features.Orders.Entities;
using Domain.Orders.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Postgreesql.Features.Orders.Configurations;

public class RefundTransactionConfiguration : IEntityTypeConfiguration<RefundTransaction>
{
    public void Configure(EntityTypeBuilder<RefundTransaction> builder)
    {
        builder.ToTable("RefundTransactions");

        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id)
            .HasColumnOrder(0)
            .UseHiLo($"Sequence-RefundTransactions");

        builder.Property(t => t.TransactionId)
            .HasColumnOrder(1)
            .IsRequired(true);

        builder.Property(t => t.OrderId)
            .HasColumnOrder(2)
            .IsRequired(true);

        builder.Property(t => t.Amount)
            .HasColumnOrder(3)
            .HasColumnName("Amount")
            .HasColumnType("numeric");

        builder.Property(t => t.CustomerWalletId)
            .HasColumnOrder(4)
            .HasColumnName("CustomerWalletId")
            .HasMaxLength(100)
            .IsRequired(true);

        builder.Property(t => t.Status)
            .HasColumnOrder(5)
            .HasColumnName("Status")
            .HasConversion(
                c => (ushort)c,
                c => (RefundTransactionStatus)c);

        builder.Property(t => t.RetryCount)
            .HasColumnOrder(6)
            .HasColumnName("RetryCount")
            .IsRequired(true);

        builder.Property(t => t.ErrorDetails)
            .HasColumnOrder(7)
            .HasColumnName("ErrorDetails")
            .HasMaxLength(500);

        builder.Property(t => t.AsaasTransferId)
            .HasColumnOrder(8)
            .HasColumnName("AsaasTransferId")
            .HasMaxLength(100);

        builder.Property(t => t.CreatedAt)
            .HasColumnOrder(9)
            .HasColumnName("Created")
            .HasColumnType("timestamp");

        builder.Property(t => t.CreatedBy)
            .HasColumnOrder(10);

        builder.Property(t => t.UpdatedAt)
            .HasColumnOrder(11)
            .HasColumnName("Updated")
            .HasColumnType("timestamp");

        builder.Property(t => t.UpdatedBy)
            .HasColumnOrder(12);

        builder.HasIndex(t => t.OrderId);
        builder.HasIndex(t => t.TransactionId);

        builder.HasOne<Order>()
            .WithMany()
            .HasForeignKey(t => t.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}