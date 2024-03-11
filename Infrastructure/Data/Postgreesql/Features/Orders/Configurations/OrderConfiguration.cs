using Infrastructure.Data.Postgreesql.Features.Orders.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Postgreesql.Features.Orders.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id)
            .UseHiLo($"Sequence-Orders");

        builder.Property(b => b.Status);

        builder.Property(x => x.Created).HasColumnName("Created").HasColumnType("timestamp");

        builder.Property(x => x.Updated)
           .HasColumnName("Updated")
           .HasDefaultValue(DateTime.Now)
           .HasColumnType("timestamp");

        builder.Property(x => x.RequestedTime)
            .HasColumnName("RequestedTime")
            .HasColumnType("timestamp");

        builder.Property(x => x.AcceptanceTime)
            .HasColumnName("AcceptanceTime")
            .HasColumnType("timestamp");

        builder.Property(x => x.CompletionTime)
            .HasColumnName("CompletionTime")
            .HasColumnType("timestamp");

        builder.Property(x => x.CancelationTime)
            .HasColumnName("CancelationTime")
            .HasColumnType("timestamp");

        builder.OwnsMany(e => e.Addresses,
            builder => { builder.ToJson(); });

        builder.HasOne(e => e.User)
            .WithMany(s => s.UserOrders)
            .HasForeignKey(e => e.UserId)
            .IsRequired(true);

        builder.HasOne(e => e.Driver)
            .WithMany(s => s.DriverOrders)
            .HasForeignKey(e => e.DriverId)
            .IsRequired(false);

        builder.Ignore(x => x.DomainEvents);
    }
}
