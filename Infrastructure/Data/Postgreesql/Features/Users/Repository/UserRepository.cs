using System.Collections.Immutable;
using Domain.Features.User.Entities;
using Domain.Features.User.Repository;
using Infrastructure.Data.Postgreesql.Features.Users.Maps;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Postgreesql.Features.Users.Repository;

public class UserRepository : IUserRepository
{
    private readonly TILTContext _context;

    public UserRepository(TILTContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<User>> GetAllAsync()
    {
        var users = await _context.Users.AsNoTracking().ToListAsync();
        var userList = users.Select(u => u.ToDomainUser()).ToImmutableList();

        return userList!;
    }

    public async Task<User?> GetByIdAsync(long id)
    {
        var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
        return user?.ToDomainUser() ?? null;
    }

    public async Task<User?> GetByEmail(string email)
    {
        var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email);
        return user?.ToDomainUser() ?? null;
    }

    public async Task<User?> GetByDocument(string document)
    {
        var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Document == document);
        return user?.ToDomainUser() ?? null;
    }

    public async Task<User> SaveAsync(User entity)
    {
        var user = entity.ToPersistanceUser();
        _context.Add(user);
        await _context.SaveChangesAsync();
        return user?.ToDomainUser()!;
    }

    public async Task<User> UpdateAsync(User entity)
    {
        var user = entity.ToPersistanceUser();
        _context.Update(user);
        await _context.SaveChangesAsync();
        return user?.ToDomainUser()!;
    }
}
