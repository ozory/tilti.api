using Domain.Features.Subscriptions.Entities;
using Domain.Subscriptions.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Postgreesql.Features.Subscriptions.Configurations;

public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.ToTable("Subscriptions");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Id)
           .HasColumnOrder(0);

        builder.HasOne(e => e.Plan)
            .WithMany(s => s.Subscriptions)
            .HasForeignKey(e => e.PlanId)
            .IsRequired(true);

        builder.Property(b => b.Status)
            .HasColumnOrder(1)
            .HasColumnName("Status")
            .HasConversion(
            c => (ushort)c,
            c => (SubscriptionStatus)c);

        builder.Property(b => b.SubscriptionType)
           .HasColumnOrder(2)
           .HasColumnName("SubscriptionType")
           .HasConversion(
            c => (int)c,
            c => (SubscriptionType)c)
           .HasDefaultValue(SubscriptionType.Driver); // NEW: Default to Driver for backward compatibility


        builder.Property(x => x.UpdatedAt)
           .HasColumnName("Updated")
           .HasColumnType("timestamp");

        builder.Property(x => x.PaymentToken)
            .HasColumnName("PaymentToken")
            .HasMaxLength(1000);

        builder.Property(x => x.AsaasPaymentId)
            .HasColumnName("AsaasPaymentId")
            .HasMaxLength(255);

        builder.Property(x => x.AsaasPaymentLink)
            .HasColumnName("AsaasPaymentLink")
            .HasColumnType("text");

        builder.Property(x => x.AsaasSubscriptionId)
            .HasColumnName("AsaasSubscriptionId")
            .HasMaxLength(255);

        builder.Property(x => x.PaidAt)
            .HasColumnName("PaidAt")
            .HasColumnType("timestamp with time zone");

        builder.HasOne(e => e.User)
            .WithOne(e => e.Subscription)
            .HasForeignKey<Subscription>(e => e.UserId);

        // NEW: Unique index for active subscriptions per user per type (temporarily removed due to filter syntax issues)
        // builder.HasIndex("UserId", "SubscriptionType")
        //     .HasFilter("\"Status\" IN (1, 3)")
        //     .IsUnique();

        builder.Ignore(x => x.DomainEvents);
    }
}
