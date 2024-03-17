using Domain.Enums;
using Domain.Features.Users.Entities;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Postgresql.Features.Users.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(b => b.Id);

        builder.Property(x => x.Id)
            .HasColumnOrder(0)
            .UseHiLo($"Sequence-Users");

        builder.Property(x => x.Name)
            .HasColumnOrder(1)
            .HasConversion(
                c => c!.Value,
                c => new Name(c!)
            )
            .HasColumnName("Name")
            .HasMaxLength(250)
            .IsRequired(true);

        builder.Property(b => b.Status)
            .HasColumnOrder(2)
            .HasColumnName("Status")
            .HasConversion(
            c => (ushort)c,
            c => (UserStatus)c);

        builder.Property(x => x.Password)
            .HasColumnOrder(3)
            .HasConversion(
                c => c!.Value,
                c => new Password(c)
            )
            .HasColumnName("Password")
            .HasMaxLength(250)
            .IsRequired(true);

        builder.Property(x => x.Document)
            .HasColumnOrder(4)
            .HasConversion(
                c => c!.Value,
                c => new Document(c!)
            )
            .HasColumnName("Document")
            .HasMaxLength(11)
            .IsRequired(true);

        builder.Property(x => x.Email)
            .HasColumnOrder(5)
            .HasConversion(
                c => c!.Value,
                c => new Email(c!)
            )
            .HasColumnName("Email")
            .HasMaxLength(250)
            .IsRequired(true);

        builder.Property(x => x.VerificationCode)
            .HasColumnOrder(6)
            .HasColumnName("ValidationCode")
            .HasMaxLength(6);

        builder.Property(x => x.Photo)
            .HasColumnOrder(7)
            .HasColumnName("Photo")
            .HasMaxLength(1000);

        builder.OwnsOne(t => t.Transport, b =>
        {
            b.Property(p => p.Name)
                .HasColumnOrder(8)
                .HasColumnName("VehicleName")
                .HasConversion(
                    c => c.Value,
                    c => new Name(c!))
                .HasMaxLength(250);

            b.Property(x => x.Description)
                .HasColumnOrder(9)
                .HasColumnName("Description")
                .HasConversion(
                    c => c.Value,
                    c => new Description(c!));

            b.Property(p => p.Model)
                .HasColumnOrder(10)
                .HasColumnName("VehicleModel")
                .HasMaxLength(250);

            b.Property(p => p.Plate)
                .HasColumnOrder(11)
                .HasColumnName("VehiclePlate")
                .HasMaxLength(250);

            b.Property(p => p.Year)
                .HasColumnOrder(12)
                .HasColumnName("VehicleYear")
                .HasColumnType("smallint");
        });

        builder.Property(x => x.CreatedAt)
            .HasColumnOrder(13)
            .HasColumnName("Created")
            .HasColumnType("timestamp");

        builder.Property(x => x.UpdatedAt)
            .HasColumnOrder(14)
           .HasColumnName("Updated")
           .HasColumnType("timestamp");

        builder.Property(x => x.VerificationSalt)
            .HasColumnOrder(15)
            .HasColumnName("ValidationSalt")
            .HasMaxLength(250);

        builder.Property(x => x.PaymentToken)
            .HasColumnOrder(16)
            .HasColumnName("PaymentToken")
            .HasMaxLength(1000);

        builder.Property(x => x.PaymentUserIdentifier)
            .HasColumnOrder(17)
            .HasColumnName("PaymentUserIdentifier")
            .HasMaxLength(1000);

        builder.HasIndex(x => new { x.Email, x.Document });

        builder.Ignore(x => x.DomainEvents);
    }
}
