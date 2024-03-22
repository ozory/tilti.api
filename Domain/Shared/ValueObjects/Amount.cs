using Domain.Abstractions;

namespace Domain.ValueObjects;

public class Amount : ValueObject<decimal>
{
    public Amount(decimal value) : base(value)
    {
    }
}