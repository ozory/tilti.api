using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Application.Shared.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using DomainUser = Domain.Features.Users.Entities.User;

namespace Application.Features.Security.Extensions;

public class SecurityExtensions : ISecurityExtensions
{
    private readonly IConfiguration _configuration;
    private string _securitySalt = null!;
    private string _securityPepper = null!;
    private int _interation;

    public SecurityExtensions(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string ComputeHash(string salt, string password)
    {
        _securitySalt = salt;
        _securityPepper = _configuration.GetValue<string>("Security:Pepper")!;
        _interation = _configuration.GetValue<int>("Security:Interation")!;

        return ComputeHash(password, _securitySalt, _securityPepper, _interation);
    }

    static private string ComputeHash(string password, string salt, string pepper, int iteration)
    {
        if (iteration <= 0) return password;

        using var sha256 = SHA256.Create();
        var passwordSaltPepper = $"{password}{salt}{pepper}";
        var byteValue = Encoding.UTF8.GetBytes(passwordSaltPepper);
        var byteHash = sha256.ComputeHash(byteValue);
        var hash = Convert.ToBase64String(byteHash);
        return ComputeHash(hash, salt, pepper, iteration - 1);
    }

    public string ComputeValidationCode()
    {
        var bytes = BitConverter.GetBytes(DateTime.Now.Ticks);
        Array.Resize(ref bytes, 16);
        var guid = new Guid(bytes);
        return guid.ToString().Substring(0, 6);
    }

    public string GenerateSalt()
    {
        using var rng = RandomNumberGenerator.Create();
        var byteSalt = new byte[16];
        rng.GetBytes(byteSalt);
        var salt = Convert.ToBase64String(byteSalt);
        return salt;
    }

    public string GenerateRefreshToken()
    {
        using var rng = RandomNumberGenerator.Create();
        var randomNumber = new byte[64];
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public string GenerateToken(DomainUser user)
    {
        var handler = new JwtSecurityTokenHandler();

        var key = Encoding.UTF8.GetBytes(_configuration.GetValue<string>("Security:Secret")!)!;
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256Signature);

        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(new Claim[]{
                    new Claim(ClaimTypes.Name, user.Email.Value!),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, "admin")
                }),
            Expires = DateTime.Now.AddHours(8),
            SigningCredentials = credentials,
            Audience = _configuration.GetValue<string>("Security:Audience")!,
            Issuer = _configuration.GetValue<string>("Security:Issuer")!,
        };

        var token = handler.CreateToken(tokenDescriptor);
        return handler.WriteToken(token);
    }

    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("Security:Secret")!)),
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken
            || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");

        return principal;

    }
}
