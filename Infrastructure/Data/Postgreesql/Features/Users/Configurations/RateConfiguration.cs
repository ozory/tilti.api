using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Postgreesql.Features.Users.Configurations;

public class RateConfiguration : IEntityTypeConfiguration<Rate>
{
    public void Configure(EntityTypeBuilder<Rate> builder)
    {
        builder.ToTable("Rates");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.SourceUserId)
            .HasColumnOrder(1)
            .IsRequired(true);

        builder.Property(b => b.TargetUserId)
            .HasColumnOrder(2)
            .IsRequired(true);

        builder.Property(x => x.Value)
            .HasColumnOrder(3)
            .HasColumnName("Value");

        builder.Property(x => x.CreatedAt)
            .HasColumnOrder(4)
            .HasColumnName("Created")
            .HasColumnType("timestamp");

        builder.Property(x => x.UpdatedAt)
            .HasColumnOrder(5)
            .HasColumnName("Updated")
            .HasColumnType("timestamp");

        builder.HasOne(e => e.SourceUser)
             .WithMany(s => s.SourceRates)
             .HasForeignKey(e => e.SourceUserId)
             .IsRequired(true);

        builder.HasOne(e => e.TargetUser)
            .WithMany(s => s.TargetRates)
            .HasForeignKey(e => e.TargetUserId)
            .IsRequired(true);

        builder.Ignore(x => x.DomainEvents);
    }
}
