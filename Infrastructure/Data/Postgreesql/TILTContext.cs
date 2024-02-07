using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Infrastructure.Data.Postgreesql.Features.Security.Entities;
using Infrastructure.Data.Postgreesql.Features.Users.Entities;
using Infrastructure.Data.Postgreesql.Migrations;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Postgreesql;

public class TILTContext : DbContext
{

    public DbSet<User> Users { get; set; }
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
