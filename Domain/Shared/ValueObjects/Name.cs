using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Abstractions;

namespace Domain.ValueObjects;

public class Name : ValueObject<string>
{
    public Name(string value) : base(value)
    {
    }
}