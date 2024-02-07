using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Abstractions;

namespace Domain.ValueObjects;

public class Document : ValueObject<string>
{
    public Document(string value) : base(value)
    {
    }
}
