using System.Reflection;
using Application.Shared.Abstractions;
using Domain.Features.Plans.Repository;
using Domain.Features.Subscription.Repository;
using Domain.Features.Users.Repository;
using Infrastructure.Data.Postgreesql.Features.Plans.Repository;
using Infrastructure.Data.Postgreesql.Features.Security.Repository;
using Infrastructure.Data.Postgreesql.Features.Subscriptions.Repository;
using Infrastructure.Data.Postgreesql.Features.Users.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Data.Postgreesql.Shared.Configurations;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddDbContext<TILTContext>(options =>
        {
            options.UseNpgsql(configuration.GetSection("Infrastructure:DataBase").Value);
        });

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPlanRepository, PlanRepository>();
        services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
        services.AddScoped<ISecurityRepository, SecurityRepository>();

        return services;
    }
}
