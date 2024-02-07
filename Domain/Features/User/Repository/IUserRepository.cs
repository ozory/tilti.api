using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Domain.Abstractions;
using DomainUser = Domain.Features.User.Entities.User;
namespace Domain.Features.User.Repository;

public interface IUserRepository : IGenericRepository<Entities.User>
{
    Task<Entities.User?> GetByEmail(string Email);
    Task<Entities.User?> GetByDocument(string Document);
}
