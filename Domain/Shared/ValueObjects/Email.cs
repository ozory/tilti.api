using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Abstractions;

namespace Domain.ValueObjects;

public class Email : ValueObject<string>
{
    public Email(string value) : base(value)
    {
    }

    public string? ValidationCode { get; set; }
}
