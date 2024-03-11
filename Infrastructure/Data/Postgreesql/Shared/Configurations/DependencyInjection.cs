using System.Globalization;
using System.Reflection;
using Application.Shared.Abstractions;
using Domain.Features.Orders.Repository;
using Domain.Features.Plans.Repository;
using Domain.Features.Subscriptions.Repository;
using Domain.Features.Users.Repository;
using Domain.Shared.Abstractions;
using Infrastructure.Data.Postgreesql.Features.Orders.Repository;
using Infrastructure.Data.Postgreesql.Features.Plans.Repository;
using Infrastructure.Data.Postgreesql.Features.Security.Repository;
using Infrastructure.Data.Postgreesql.Features.Subscriptions.Repository;
using Infrastructure.Data.Postgreesql.Features.Users.Repository;
using Infrastructure.Data.Postgreesql.Shared.Abstractions;
using Infrastructure.External.Orders.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
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
        services.AddScoped<IOrderRepository, OrdersRepository>();
        services.AddScoped<ISecurityRepository, SecurityRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetSection("Infrastructure:Redis:Server").Value;
            options.InstanceName = configuration.GetSection("Infrastructure:Redis:InstanceName").Value;
        });

        services.AddSingleton<IMapServices>(sp =>
        {
            var valuePerKM = Decimal.Parse(
            configuration.GetSection("Configurations:PricePerKM").Value!,
            CultureInfo.InvariantCulture);

            var valuePerDuration = Decimal.Parse(
                configuration.GetSection("Configurations:PricePerDuration").Value!,
                CultureInfo.InvariantCulture);

            var apiKey = configuration.GetSection("Configurations:ApiKey").Value;
            var dc = sp.GetRequiredService<IDistributedCache>();

            return new MapServices(valuePerKM, valuePerDuration, apiKey!, dc);
        });

        return services;
    }
}
