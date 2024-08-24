using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Orders.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Postgreesql.Features.Orders.Configurations;

public class MessagesConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable("Messages");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Id)
          .HasColumnOrder(0);

        builder.Property(b => b.OrderId)
           .HasColumnOrder(1)
           .IsRequired(true);

        builder.Property(b => b.SourceUserId)
           .HasColumnOrder(2)
           .IsRequired(true);

        builder.Property(b => b.TargetUserId)
            .HasColumnOrder(3)
            .IsRequired(true);

        builder.Property(x => x.CreatedAt)
           .HasColumnOrder(4)
           .HasColumnName("Created")
           .HasColumnType("timestamp");

        builder.Property(x => x.UpdatedAt)
            .HasColumnOrder(5)
            .HasColumnName("Updated")
            .HasColumnType("timestamp");

        builder.HasOne(e => e.SourceUser)
            .WithMany(s => s.SourceMessages)
            .HasForeignKey(e => e.SourceUserId)
            .IsRequired(true);

        builder.HasOne(e => e.TargetUser)
            .WithMany(s => s.TargetMessages)
            .HasForeignKey(e => e.TargetUserId)
            .IsRequired(true);

        builder.HasOne(e => e.Order)
            .WithMany(s => s.Messages)
            .HasForeignKey(e => e.OrderId)
            .IsRequired(true);

        builder.Ignore(x => x.DomainEvents);
    }
}
