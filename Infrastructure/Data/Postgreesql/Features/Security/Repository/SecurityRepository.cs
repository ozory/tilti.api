using Application.Shared.Abstractions;
using Infrastructure.Data.Postgreesql.Features.Security.Maps;
using Microsoft.EntityFrameworkCore;
using RefreshTokens = Application.Features.Security.Contracts.RefreshTokens;

namespace Infrastructure.Data.Postgreesql.Features.Security.Repository
{
    public class SecurityRepository : ISecurityRepository
    {
        private readonly TILTContext _context;

        public SecurityRepository(TILTContext context)
        {
            _context = context;
        }

        public async Task<RefreshTokens?> GetRefreshTokenAsync(string token)
        {
            var rf = await _context.RefreshTokens.AsNoTracking().FirstOrDefaultAsync(u => u.RefreshToken == token);
            return rf?.ToAppRefreshToken() ?? null;
        }

        public async Task DeleteToken(long userId)
        {
            var token = await _context.RefreshTokens.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == userId);
            if (token is not null)
            {
                _context.RefreshTokens.Remove(token);
                await _context.SaveChangesAsync();
            }
        }

        public async Task SaveToken(RefreshTokens token)
        {
            await _context.RefreshTokens.AddAsync(token.ToPersistanceRefreshToken());
            await _context.SaveChangesAsync();
        }
    }
}