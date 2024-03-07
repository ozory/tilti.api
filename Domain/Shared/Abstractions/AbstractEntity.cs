using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Shared.Abstractions;

public abstract class AbstractEntity
{
    public virtual List<IDomainEvent> DomainEvents { get; protected set; } = new();

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
