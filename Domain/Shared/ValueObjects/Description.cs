using Domain.Abstractions;

namespace Domain.ValueObjects;

public class Description : ValueObject<string>
{
    public Description(string value) : base(value)
    {
    }
}