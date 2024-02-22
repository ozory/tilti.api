using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Data.Postgreesql.Features.Orders.Entities;

namespace Infrastructure.Data.Postgreesql.Features.Users.Entities;

public class User
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
}
