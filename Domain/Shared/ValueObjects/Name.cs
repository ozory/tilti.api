using Domain.Abstractions;

namespace Domain.ValueObjects;

public class Name : ValueObject<string>
{
    public Name(string value) : base(value)
    {
    }
}