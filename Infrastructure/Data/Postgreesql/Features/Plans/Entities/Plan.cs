using Infrastructure.Data.Postgreesql.Features.Subscriptions.Entities;

namespace Infrastructure.Data.Postgreesql.Features.Plans.Entities;

public class Plan
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;

    public Decimal Amount { get; set; }
    public ushort Status { get; set; }

    public DateTime Created { get; set; }
    public DateTime Updated { get; set; } = DateTime.Now;

    public ICollection<Subscription>? Subscriptions { get; } = [];

}
