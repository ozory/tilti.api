using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Abstractions;

namespace Domain.ValueObjects;

public class Description : ValueObject<string>
{
    public Description(string value) : base(value)
    {
    }
}