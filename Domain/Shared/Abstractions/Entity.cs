using Domain.Shared.Abstractions;
using FluentResults;

namespace Domain.Abstractions;

public abstract class Entity
{
    public long Id { get; protected set; }
    public DateTime CreatedAt { get; protected set; }
    public long CreatedBy { get; protected set; }
    public DateTime UpdatedAt { get; protected set; }
    public long? UpdatedBy { get; protected set; }
    public virtual List<IDomainEvent> DomainEvents { get; protected set; } = new();

    public virtual void SetCreated(DateTime? dateTime)
    {
        this.CreatedAt = dateTime ?? DateTime.Now;
    }

    public virtual void SetUpdated(DateTime? dateTime)
    {
        this.UpdatedAt = dateTime ?? DateTime.Now;
    }

    public virtual Result<Entity> AddError(string message)
    {
        var i = this.ToResult().WithError(new Error(message));
        return i.ToResult<Entity>();
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
