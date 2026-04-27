using Domain.Features.Subscriptions.Entities;
using Domain.Subscriptions.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Postgreesql.Features.Subscriptions.Configurations;

/// <summary>
/// EF Core configuration for DriverSubscription entity
/// </summary>
public class DriverSubscriptionConfiguration : IEntityTypeConfiguration<DriverSubscription>
{
    public void Configure(EntityTypeBuilder<DriverSubscription> builder)
    {
        builder.ToTable("driver_subscriptions");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Id)
           .HasColumnOrder(0);

        builder.HasOne(e => e.Plan)
            .WithMany() // Plan doesn't have a collection of DriverSubscriptions defined
            .HasForeignKey(e => e.PlanId)
            .IsRequired(true)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.User)
            .WithMany() // User doesn't have a collection of DriverSubscriptions defined
            .HasForeignKey(e => e.UserId)
            .IsRequired(true)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(b => b.Status)
            .HasColumnOrder(1)
            .HasColumnName("status")
            .HasMaxLength(50)
            .HasConversion(
                c => c.ToString(),
                c => Enum.Parse<SubscriptionStatus>(c));

        builder.Property(x => x.DueDate)
            .HasColumnName("due_date")
            .HasColumnType("timestamp with time zone");

        builder.Property(x => x.PaymentToken)
            .HasColumnName("payment_token")
            .HasMaxLength(255);

        builder.Property(x => x.AsaasPaymentId)
            .HasColumnName("asaas_payment_id")
            .HasMaxLength(255);

        builder.Property(x => x.AsaasPaymentLink)
            .HasColumnName("asaas_payment_link")
            .HasColumnType("text");

        builder.Property(x => x.AsaasSubscriptionId)
            .HasColumnName("asaas_subscription_id")
            .HasMaxLength(255);

        builder.Property(x => x.PaidAt)
            .HasColumnName("paid_at")
            .HasColumnType("timestamp with time zone");

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp with time zone");

        builder.Property(x => x.UpdatedAt)
           .HasColumnName("updated_at")
           .HasColumnType("timestamp with time zone");

        // Index for fast user+status queries (ride eligibility check)
        builder.HasIndex("UserId", "Status")
            .HasDatabaseName("IX_driver_subscriptions_user_id_status");
    }
}