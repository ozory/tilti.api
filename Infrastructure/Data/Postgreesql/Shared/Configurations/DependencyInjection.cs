using System.Reflection;
using Domain.Features.Subscription.Repository;
using Domain.Features.User.Repository;
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
        services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
        //services.AddScoped<IPlanRepository, PlanRepository>();
        // services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        return services;
    }
}
