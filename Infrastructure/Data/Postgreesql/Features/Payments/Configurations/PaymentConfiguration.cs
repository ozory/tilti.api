using Domain.Features.Payments.Entities;
using Domain.Features.Payments.Enums;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Postgreesql.Features.Payments.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .UseHiLo($"Sequence-Payments");

        builder.Property(p => p.UserId)
            .IsRequired(true);

        builder.Property(p => p.ReceiverId)
            .IsRequired(true);

        builder.Property(p => p.Amount)
            .HasColumnName("Amount")
            .HasConversion(
                c => c.Value,
                c => new Amount(c));

        builder.Property(p => p.Status)
            .HasColumnName("Status")
            .HasConversion(
                c => (ushort)c,
                c => (PaymentStatus)c);

        builder.Property(p => p.Type)
            .HasColumnName("Type")
            .HasConversion(
                c => (ushort)c,
                c => (PaymentType)c);

        builder.Property(p => p.AsaasPaymentId)
            .HasColumnName("AsaasPaymentId")
            .HasColumnType("text");

        builder.Property(p => p.PixQrCode)
            .HasColumnName("PixQrCode")
            .HasColumnType("text");

        builder.Property(p => p.PixLink)
            .HasColumnName("PixLink")
            .HasColumnType("text");

        builder.Property(p => p.OrderId)
            .HasColumnName("OrderId");

        builder.Property(p => p.ApprovedAt)
            .HasColumnName("ApprovedAt")
            .HasColumnType("timestamp with time zone");

        builder.Property(p => p.CancelledAt)
            .HasColumnName("CancelledAt")
            .HasColumnType("timestamp with time zone");

        builder.HasIndex(p => p.AsaasPaymentId);
        builder.HasIndex(p => p.OrderId);
        builder.HasIndex(p => p.UserId);
    }
}