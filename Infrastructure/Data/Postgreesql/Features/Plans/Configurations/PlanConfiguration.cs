using Domain.Features.Plans.Entities;
using Domain.Plans.Enums;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Infrastructure.Data.Postgresql.Features.Plans.Configurations;

public class PlanConfiguration : IEntityTypeConfiguration<Plan>
{
    public void Configure(EntityTypeBuilder<Plan> builder)
    {
        builder.ToTable("Plans");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Id)
           .HasColumnOrder(0);

        builder.Property(b => b.Status)
            .HasColumnOrder(1)
            .HasColumnName("Status")
            .HasConversion(
            c => (ushort)c,
            c => (PlanStatus)c);

        builder.Property(x => x.Name)
            .HasColumnOrder(2)
            .HasColumnName("Name")
            .HasMaxLength(250)
            .HasConversion(
            c => c.Value,
            c => new Name(c!))
            .IsRequired(true);

        builder.Property(x => x.Description)
            .HasColumnOrder(3)
            .HasColumnName("Description")
            .HasConversion(
            c => c.Value,
            c => new Description(c!))
            .IsRequired(true);

        builder.Property(x => x.Amount)
            .HasColumnOrder(4)
            .HasColumnName("Amount")
             .HasConversion(
                c => c.Value,
                c => new Amount(c))
            .IsRequired(true);

        builder.Property(x => x.CreatedAt)
            .HasColumnOrder(5)
            .HasColumnName("Created")
            .HasColumnType("timestamp");

        builder.Property(x => x.UpdatedAt)
            .HasColumnOrder(6)
            .HasColumnName("Updated")
            .HasColumnType("timestamp");

        builder.HasMany(s => s.Subscriptions)
            .WithOne(p => p.Plan)
            .HasForeignKey(p => p.PlanId)
            .IsRequired(false);

        builder.Ignore(x => x.DomainEvents);

    }
}
