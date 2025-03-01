using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Orders.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Postgreesql.Features.Orders.Configurations;

public class TrackingConfiguration : IEntityTypeConfiguration<Tracking>
{
    public void Configure(EntityTypeBuilder<Tracking> builder)
    {
        builder.ToTable("Trackings");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Id)
          .HasColumnOrder(0);

        builder.Property(b => b.OrderId)
          .HasColumnOrder(1)
          .IsRequired(true);

        builder.Property(x => x.CreatedAt)
           .HasColumnOrder(2)
           .HasColumnName("Created")
           .HasColumnType("timestamp");

        builder.Property(b => b.Point)
            .HasColumnType("geography(POINT, 4326)")
            .HasColumnName("Location")
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .HasColumnOrder(3)
            .HasColumnName("Updated")
            .HasColumnType("timestamp");

        builder.HasOne(e => e.Order)
            .WithMany(s => s.Trackings)
            .HasForeignKey(e => e.OrderId)
            .IsRequired(true);

        builder.Ignore(x => x.DomainEvents);
        builder.Ignore(x => x.Location);
    }
}
