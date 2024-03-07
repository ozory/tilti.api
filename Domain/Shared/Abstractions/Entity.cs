using Domain.Shared.Abstractions;
using FluentResults;

namespace Domain.Abstractions;

public abstract class Entity : AbstractEntity
{
    public long Id { get; protected set; }
    public DateTime CreatedAt { get; protected set; }
    public long CreatedBy { get; protected set; }
    public DateTime UpdatedAt { get; protected set; }
    public long? UpdatedBy { get; protected set; }

    public virtual Result<Entity> AddError(string message)
    {
        var i = this.ToResult().WithError(new Error(message));
        return i.ToResult<Entity>();
    }
}
