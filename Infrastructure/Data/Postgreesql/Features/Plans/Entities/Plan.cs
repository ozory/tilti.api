using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Data.Postgreesql.Features.Plans.Entities;

public class Plan
{
    // Campos de usuário
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;

    public Decimal Amount { get; set; }
    public ushort Status { get; set; }

    public DateTime Created { get; set; }
    public DateTime Updated { get; set; } = DateTime.Now;

}
