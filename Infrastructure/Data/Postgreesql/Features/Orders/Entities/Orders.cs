using Infrastructure.Data.Postgreesql.Features.Users.Entities;

namespace Infrastructure.Data.Postgreesql.Features.Orders.Entities;

public class Order
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public long? DriverId { get; set; }
    public ushort Status { get; set; }
    public decimal Amount { get; set; }

    public DateTime Created { get; set; }
    public DateTime Updated { get; set; } = DateTime.Now;

    public DateTime? RequestedTime { get; set; }
    public DateTime? AcceptanceTime { get; set; }
    public DateTime? CompletionTime { get; set; }
    public DateTime? CancelationTime { get; set; }

    public List<Address> Addresses { get; } = [];

    public User User { get; set; } = null!;
    public User? Driver { get; set; }


}
