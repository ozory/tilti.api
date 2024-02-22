using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Data.Postgreesql.Features.Plans.Entities;
using Infrastructure.Data.Postgreesql.Features.Users.Entities;

namespace Infrastructure.Data.Postgreesql.Features.Subscriptions.Entities;

public class Subscription
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public long PlanId { get; set; }
    public ushort Status { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; } = DateTime.Now;

    public User User { get; set; } = null!;
    public Plan Plan { get; set; } = null!;
}
