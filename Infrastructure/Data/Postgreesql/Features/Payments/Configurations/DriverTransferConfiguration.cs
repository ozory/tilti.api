using Domain.Features.Payments.Entities;
using Domain.Features.Payments.Enums;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Postgreesql.Features.Payments.Configurations;

public class DriverTransferConfiguration : IEntityTypeConfiguration<DriverTransfer>
{
    public void Configure(EntityTypeBuilder<DriverTransfer> builder)
    {
        builder.ToTable("DriverTransfers");

        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id)
            .HasColumnOrder(0)
            .UseHiLo($"Sequence-DriverTransfers");

        builder.Property(t => t.OrderId)
            .HasColumnOrder(1)
            .IsRequired(true);

        builder.Property(t => t.DriverId)
            .HasColumnOrder(2)
            .IsRequired(true);

        builder.Property(t => t.Amount)
            .HasColumnOrder(3)
            .HasColumnName("Amount")
            .HasConversion(
                c => c.Value,
                c => new Amount(c));

        builder.Property(t => t.Status)
            .HasColumnOrder(4)
            .HasColumnName("Status")
            .HasConversion(
                c => (ushort)c,
                c => (TransferStatus)c);

        builder.Property(t => t.AsaasTransferId)
            .HasColumnOrder(5)
            .HasColumnName("AsaasTransferId")
            .HasMaxLength(100);

        builder.Property(t => t.CompletedAt)
            .HasColumnOrder(6)
            .HasColumnName("CompletedAt")
            .HasColumnType("timestamp");

        builder.Property(t => t.FailedAt)
            .HasColumnOrder(7)
            .HasColumnName("FailedAt")
            .HasColumnType("timestamp");

        builder.Property(t => t.ErrorMessage)
            .HasColumnOrder(8)
            .HasColumnName("ErrorMessage")
            .HasMaxLength(500);

        builder.Property(x => x.CreatedAt)
            .HasColumnOrder(9)
            .HasColumnName("Created")
            .HasColumnType("timestamp");

        builder.Property(x => x.UpdatedAt)
            .HasColumnOrder(10)
            .HasColumnName("Updated")
            .HasColumnType("timestamp");

        // Relationships
        builder.HasOne(t => t.Order)
            .WithMany()
            .HasForeignKey(t => t.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(t => t.Driver)
            .WithMany()
            .HasForeignKey(t => t.DriverId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}