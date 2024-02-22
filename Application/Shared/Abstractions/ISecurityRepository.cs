using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Security.Contracts;

namespace Application.Shared.Abstractions;

public interface ISecurityRepository
{
    Task<RefreshTokens?> GetRefreshTokenAsync(string token);

    Task DeleteToken(long userId);

    Task SaveToken(RefreshTokens token);
}
