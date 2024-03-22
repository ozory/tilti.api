using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Orders.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Postgreesql.Features.Orders.Configurations;

public class RejectionConfiguration : IEntityTypeConfiguration<Rejection>
{
    public void Configure(EntityTypeBuilder<Rejection> builder)
    {
        builder.ToTable("Rejections");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Id)
          .HasColumnOrder(0);

        builder.Property(b => b.DriverId)
            .HasColumnOrder(1)
            .IsRequired(true);

        builder.Property(b => b.OrderId)
            .HasColumnOrder(2)
            .IsRequired(true);

        builder.Property(x => x.CreatedAt)
            .HasColumnOrder(3)
            .HasColumnName("Created")
            .HasColumnType("timestamp");

        builder.Property(x => x.UpdatedAt)
            .HasColumnOrder(4)
            .HasColumnName("Updated")
            .HasColumnType("timestamp");

        builder.HasOne(e => e.User)
            .WithMany(s => s.Rejections)
            .HasForeignKey(e => e.DriverId)
            .IsRequired(true);

        builder.HasOne(e => e.Order)
            .WithMany(s => s.Rejections)
            .HasForeignKey(e => e.OrderId)
            .IsRequired();

        builder.Ignore(x => x.DomainEvents);
    }
}
