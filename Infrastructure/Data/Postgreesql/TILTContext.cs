using System.Reflection;
using Domain.Features.Orders.Entities;
using Domain.Features.Plans.Entities;
using Domain.Features.Subscriptions.Entities;
using Domain.Features.Users.Entities;
using Infrastructure.Data.Postgreesql.Features.Security.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Postgreesql;

public class TILTContext : DbContext
{

    public DbSet<User> Users { get; set; }
    public DbSet<Plan> Plans { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<RefreshTokens> RefreshTokens { get; set; }

    public TILTContext(DbContextOptions<TILTContext> options) : base(options)
    {

    }

    override protected void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("tilt");
    }
}
