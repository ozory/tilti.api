using Domain.Shared.Abstractions;
using FluentResults;

namespace Domain.Abstractions;

public abstract class Entity<T>
{
    public long Id { get; protected set; }
    public DateTime CreatedAt { get; protected set; }
    public long CreatedBy { get; protected set; }
    public DateTime UpdatedAt { get; protected set; }
    public long? UpdatedBy { get; protected set; }

    public virtual List<IDomainEvent> DomainEvents { get; protected set; } = new();

    public virtual Result<T> AddError(string message)
    {
        var i = this.ToResult().WithError(new Error(message));
        return i.ToResult<T>();
    }

    public virtual void AddDomainEvent(IDomainEvent eventItem)
    {
        DomainEvents.Add(eventItem);
    }

    public virtual void RemoveDomainEvent(IDomainEvent eventItem)
    {
        DomainEvents.Remove(eventItem);
    }

    public virtual void ClearEvents()
    {
        DomainEvents.Clear();
    }
}
