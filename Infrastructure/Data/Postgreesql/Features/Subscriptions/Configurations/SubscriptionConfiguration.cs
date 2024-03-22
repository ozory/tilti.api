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

        builder.Property(x => x.DueDate)
            .HasColumnName("DueDate")
            .HasColumnType("timestamp");

        builder.Property(x => x.CreatedAt)
            .HasColumnName("Created")
            .HasColumnType("timestamp");

        builder.Property(x => x.UpdatedAt)
           .HasColumnName("Updated")
           .HasColumnType("timestamp");

        builder.Property(x => x.PaymentToken)
            .HasColumnName("PaymentToken")
            .HasMaxLength(1000);

        builder.HasOne(e => e.User)
            .WithOne(e => e.Subscription)
            .HasForeignKey<Subscription>(e => e.UserId);

        builder.Ignore(x => x.DomainEvents);
    }
}
