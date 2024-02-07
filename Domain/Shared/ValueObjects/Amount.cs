using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Abstractions;

namespace Domain.ValueObjects;

public class Amount : ValueObject<decimal>
{
    public Amount(decimal value) : base(value)
    {
    }
}