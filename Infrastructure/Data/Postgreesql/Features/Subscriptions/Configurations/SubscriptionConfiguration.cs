using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InfrastructureSub = Infrastructure.Data.Postgreesql.Features.Subscriptions.Entities.Subscription;

namespace Infrastructure.Data.Postgreesql.Features.Subscriptions.Configurations;

public class SubscriptionConfiguration : IEntityTypeConfiguration<InfrastructureSub>
{
    public void Configure(EntityTypeBuilder<InfrastructureSub> builder)
    {
        builder.ToTable("Subscriptions");

        builder.HasKey(b => b.Id);

        builder.Property(x => x.DueDate)
            .HasColumnName("DueDate")
            .HasColumnType("timestamp");

        builder.Property(x => x.Created)
            .HasColumnName("Created")
            .HasColumnType("timestamp");

        builder.Property(x => x.Updated)
           .HasColumnName("Updated")
           .HasDefaultValue(DateTime.Now)
           .HasColumnType("timestamp");

        builder.HasOne(e => e.User)
            .WithOne();

        builder.HasOne(e => e.Plan)
            .WithOne();

        builder.Property(b => b.Status);
    }
}
