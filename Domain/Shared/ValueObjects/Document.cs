using Domain.Abstractions;

namespace Domain.ValueObjects;

public class Document : ValueObject<string>
{
    public Document(string value) : base(value)
    {
    }
}
