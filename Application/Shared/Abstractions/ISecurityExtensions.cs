using System.Security.Claims;
using DomainUser = Domain.Features.Users.Entities.User;

namespace Application.Shared.Abstractions;

public interface ISecurityExtensions
{
    string ComputeHash(string salt, string password);

    string ComputeValidationCode();

    string GenerateSalt();

    string GenerateToken(DomainUser user);

    string GenerateRefreshToken();

    ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token);

}