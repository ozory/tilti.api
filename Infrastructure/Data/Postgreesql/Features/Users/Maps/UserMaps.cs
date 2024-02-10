using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Enums;
using Infrastructure.Data.Postgreesql.Features.Users.Entities;
using DomainUser = Domain.Features.Users.Entities.User;

namespace Infrastructure.Data.Postgreesql.Features.Users.Maps;

public static class UserMaps
{
    internal static User ToPersistanceUser(this DomainUser user)
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
        };

        return persistanceUser;
    }

    internal static DomainUser ToDomainUser(this User user)
    {
        var domainUser = DomainUser.CreateUser(
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

        return domainUser;
    }
}
