using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Abstractions;

public abstract class ValueObject<T>
{
    public T? Value { get; protected set; }

    public ValueObject(T? value)
    {
        this.SetValue(value);
    }

    public ValueObject()
    {
    }

    public virtual void SetValue(T? value)
    {
        this.Value = value;
    }

    public virtual bool Validate()
    {
        return true;
    }
}
