using System.Globalization;
using System.Reflection;
using Application.Features.Users.Consumers;
using Application.Shared.Abstractions;
using Domain.Features.Orders.Repository;
using Domain.Features.Plans.Repository;
using Domain.Features.Subscriptions.Repository;
using Domain.Features.Users.Repository;
using Domain.Shared.Abstractions;
using Infrastructure.Cache;
using Infrastructure.Data.Postgreesql.Features.Orders.Repository;
using Infrastructure.Data.Postgreesql.Features.Plans.Repository;
using Infrastructure.Data.Postgreesql.Features.Security.Repository;
using Infrastructure.Data.Postgreesql.Features.Subscriptions.Repository;
using Infrastructure.Data.Postgreesql.Features.Users.Repository;
using Infrastructure.Data.Postgreesql.Shared.Abstractions;
using Infrastructure.External.Features.Maps.Services;
using Infrastructure.Messages;
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
            options.UseNpgsql(
                configuration.GetSection("Infrastructure:DataBase").Value,
                o => o.UseNetTopologySuite());
        });

        services.AddScoped<IUserRepository, UserRepository>();
<<<<<<< HEAD
=======
        services.AddScoped<IRateRepository, RateRepository>();
>>>>>>> 7b148c2b8ae7f452586616b92a970a41fd43347c
        services.AddScoped<IPlanRepository, PlanRepository>();
        services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
        services.AddScoped<IOrderRepository, OrdersRepository>();
        services.AddScoped<ISecurityRepository, SecurityRepository>();
        services.AddScoped<IRejectRepository, RejectionRepository>();
<<<<<<< HEAD
=======
        services.AddScoped<IOrderMessageRepository, OrderMessageRepository>();
        services.AddScoped<ITrackingRepository, TrackingRepository>();
>>>>>>> 7b148c2b8ae7f452586616b92a970a41fd43347c

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddSingleton<ICacheRepository, CacheRepository>();

<<<<<<< HEAD
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetSection("Infrastructure:Redis:Server").Value;
            options.InstanceName = configuration.GetSection("Infrastructure:Redis:InstanceName").Value;
        });

=======
>>>>>>> 7b148c2b8ae7f452586616b92a970a41fd43347c
        services.AddSingleton<IMapServices>(sp =>
        {
            var valuePerKM = Decimal.Parse(
            configuration.GetSection("Configurations:PricePerKM").Value!,
            CultureInfo.InvariantCulture);

            var valuePerDuration = Decimal.Parse(
                configuration.GetSection("Configurations:PricePerDuration").Value!,
                CultureInfo.InvariantCulture);

            var apiKey = configuration.GetSection("Configurations:ApiKey").Value;
            var dc = sp.GetRequiredService<ICacheRepository>();

            return new MapServices(valuePerKM, valuePerDuration, apiKey!, dc);
        });

        services.AddScoped<IMessageRepository, MessageRepository>();

        return services;
    }
}
