using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Abstractions;

namespace Domain.ValueObjects;

public class Password : ValueObject<string>
{
    public Password(string? value) : base(value)
    {
    }
}
