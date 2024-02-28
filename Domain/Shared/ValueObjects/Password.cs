using Domain.Abstractions;

namespace Domain.ValueObjects;

public class Password : ValueObject<string>
{
    public Password(string? value) : base(value)
    {
    }
}
