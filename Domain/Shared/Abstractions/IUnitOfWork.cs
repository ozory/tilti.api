using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Abstractions;

namespace Domain.Shared.Abstractions;

public interface IUnitOfWork
{
    public IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : Entity;

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
