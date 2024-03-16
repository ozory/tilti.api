using Domain.Enums;
using Domain.Features.Orders.Entities;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetTopologySuite.Geometries;

namespace Infrastructure.Data.Postgreesql.Features.Orders.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id)
            .HasColumnOrder(0)
            .UseHiLo($"Sequence-Orders");

        builder.Property(b => b.UserId)
            .HasColumnOrder(1)
            .IsRequired(true);

        builder.Property(b => b.DriverId)
            .HasColumnOrder(2)
            .IsRequired(false);

        builder.Property(b => b.Status)
            .HasColumnOrder(3)
            .HasColumnName("Status")
            .HasConversion(
            c => (ushort)c,
            c => (OrderStatus)c);

        builder.Property(b => b.Amount)
            .HasColumnOrder(4)
            .HasColumnName("Amount")
            .HasConversion(
            c => c.Value,
            c => new Amount(c));

        builder.Property(x => x.CreatedAt)
            .HasColumnOrder(5)
            .HasColumnName("Created")
            .HasColumnType("timestamp");

        builder.Property(x => x.UpdatedAt)
            .HasColumnOrder(6)
           .HasColumnName("Updated")
           .HasDefaultValue(DateTime.Now)
           .HasColumnType("timestamp");

        builder.Property(x => x.RequestedTime)
            .HasColumnOrder(7)
            .HasColumnName("RequestedTime")
            .HasColumnType("timestamp");

        builder.Property(x => x.AcceptanceTime)
            .HasColumnOrder(8)
            .HasColumnName("AcceptanceTime")
            .HasColumnType("timestamp");

        builder.Property(x => x.CompletionTime)
            .HasColumnOrder(9)
            .HasColumnName("CompletionTime")
            .HasColumnType("timestamp");

        builder.Property(x => x.CancelationTime)
            .HasColumnOrder(10)
            .HasColumnName("CancelationTime")
            .HasColumnType("timestamp");

        builder.Property(b => b.Location)
            .HasColumnType("geography(POINT, 4326)")
             .HasConversion(
                    c => new Point(c!.Latitude, c!.Longitude),
                    c => new Domain.Shared.ValueObjects.Position()
                    {
                        Latitude = c.Coordinate.X,
                        Longitude = c.Coordinate.Y
                    })
            .IsRequired(false);

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

        builder.Ignore(x => x.Items);
        builder.Ignore(x => x.DomainEvents);
        //builder.Ignore(x => x.Location);
    }
}
