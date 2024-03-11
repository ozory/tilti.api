using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InfrastructureUser = Infrastructure.Data.Postgreesql.Features.Users.Entities.User;


namespace Infrastructure.Data.Postgresql.Features.Users.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<InfrastructureUser>
{
    public void Configure(EntityTypeBuilder<InfrastructureUser> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(b => b.Id);

        builder.Property(x => x.Id)
            .UseHiLo($"Sequence-Users");

        builder.Property(x => x.Name)
            .HasColumnName("Name")
            .HasMaxLength(250)
            .IsRequired(true);

        builder.Property(x => x.Password)
            .HasColumnName("Password")
            .HasMaxLength(250)
            .IsRequired(true);

        builder.Property(x => x.Document)
            .HasColumnName("Document")
            .HasMaxLength(11)
            .IsRequired(true);

        builder.Property(x => x.Email)
            .HasColumnName("Email")
            .HasMaxLength(250)
            .IsRequired(true);

        builder.Property(x => x.ValidationCode)
            .HasColumnName("ValidationCode")
            .HasMaxLength(6);

        builder.Property(x => x.Photo)
            .HasColumnName("Photo")
            .HasMaxLength(1000);

        builder.Property(x => x.VehicleModel)
            .HasColumnName("VehicleModel")
            .HasMaxLength(250);

        builder.Property(x => x.VehiclePlate)
           .HasColumnName("VehiclePlate")
           .HasMaxLength(250);

        builder.Property(x => x.VehicleYear)
            .HasColumnName("VehicleYear")
            .HasColumnType("smallint");

        builder.Property(x => x.Created)
            .HasColumnName("Created")
            .HasColumnType("timestamp");

        builder.Property(x => x.Updated)
           .HasColumnName("Updated")
           .HasDefaultValue(DateTime.Now)
           .HasColumnType("timestamp");

        builder.Property(x => x.ValidationSalt)
            .HasColumnName("ValidationSalt")
            .HasMaxLength(250);

        builder.Property(x => x.PaymentToken)
            .HasColumnName("PaymentToken")
            .HasMaxLength(1000);

        builder.Ignore(x => x.DomainEvents);

        builder.Property(b => b.Status);

    }
}
