using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InfrastructurePlan = Infrastructure.Data.Postgreesql.Features.Plans.Entities.Plan;


namespace Infrastructure.Data.Postgresql.Features.Plans.Configurations;

public class PlanConfiguration : IEntityTypeConfiguration<InfrastructurePlan>
{
    public void Configure(EntityTypeBuilder<InfrastructurePlan> builder)
    {
        builder.ToTable("Plans");

        builder.HasKey(b => b.Id);

        builder.Property(x => x.Name)
            .HasColumnName("Name")
            .HasMaxLength(250)
            .IsRequired(true);

        builder.Property(x => x.Description)
            .HasColumnName("Description")
            .IsRequired(true);

        builder.Property(x => x.Amount)
            .HasColumnName("Amount")
            .IsRequired(true);

        builder.Property(x => x.Created)
            .HasColumnName("Created")
            .HasColumnType("timestamp");

        builder.Property(x => x.Updated)
           .HasColumnName("Updated")
           .HasDefaultValue(DateTime.Now)
           .HasColumnType("timestamp");

        builder.Property(b => b.Status);

    }
}
