using Domain.Abstractions;
using Domain.Enums;
using Domain.Shared.Abstractions;
using Infrastructure.Data.Postgreesql.Features.Orders.Entities;
using Infrastructure.Data.Postgreesql.Shared.Abstractions;
using Npgsql.Replication;
using DomainUser = Domain.Features.Users.Entities.User;
namespace Infrastructure.Data.Postgreesql.Features.Users.Entities;

public class User : InfrastructureEntity
{
    // Campos de usuário
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public bool DriveEnable { get; set; } = false;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Document { get; set; } = null!;
    public string? ValidationCode { get; set; }
    public string? ValidationSalt { get; set; }
    public string? PaymentToken { get; set; }

    public ushort Status { get; set; }
    public string? Photo { get; set; } = null!;

    public DateTime Created { get; set; }
    public DateTime Updated { get; set; } = DateTime.Now;

    // Campos de transportador
    public string? VehiclePlate { get; set; }
    public string? VehicleModel { get; set; }
    public ushort? VehicleYear { get; set; }
    public string? VerificationCode { get; set; }

    public ICollection<Order>? UserOrders { get; } = [];
    public ICollection<Order>? DriverOrders { get; } = [];

    public static explicit operator User(DomainUser user)
    {
        User persistanceUser = new User
        {
            Id = user.Id,
            Name = user.Name.Value!,
            Document = user.Document.Value!,
            Email = user.Email.Value!,
            Password = user.Password!.Value!,
            Status = (ushort)user.Status,
            Created = user.CreatedAt,

            Photo = user.Photo,
            VehicleModel = user.Transport?.Model,
            VehiclePlate = user.Transport?.Plate,
            VehicleYear = user.Transport?.Year,

            VerificationCode = user.VerificationCode,
            ValidationSalt = user.VerificationSalt,
            DriveEnable = user.DriveEnable,
            PaymentToken = user.PaymentToken,

            // DomainEvents
            DomainEvents = user.DomainEvents
        };

        return persistanceUser;
    }

    public static implicit operator DomainUser(User? user)
    {
        if (user is null) return null!;
        var domainUser = DomainUser.Create(
            user.Id,
            user.Name,
            user.Email,
            user.Document,
            null,
            user.Created);

        domainUser.SetUpdatedAt(user.Updated);
        domainUser.SetVerificationSalt(user.ValidationSalt!);
        domainUser.SetStatus((UserStatus)user.Status);
        domainUser.SetDriveEnable(user.DriveEnable);
        domainUser.SetPaymentToken(user.PaymentToken);

        return domainUser;
    }
}
