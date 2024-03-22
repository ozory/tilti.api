using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Shared.ValueObjects;

namespace Domain.Shared.Abstractions;

public interface IGeoData
{
    public long UserId { get; protected set; }
    public Location Location { get; protected set; }
}
