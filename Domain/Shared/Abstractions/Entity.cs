using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentResults;
using FluentValidation;
using FluentValidation.Validators;

namespace Domain.Abstractions;

public abstract class Entity<T>
{
    public long Id { get; protected set; }
    public DateTime CreatedAt { get; protected set; }
    public long CreatedBy { get; protected set; }
    public DateTime UpdatedAt { get; protected set; }
    public long? UpdatedBy { get; protected set; }

    public virtual Result<T> AddError(string message)
    {
        var i = this.ToResult().WithError(new Error(message));
        return i.ToResult<T>();
    }
}
