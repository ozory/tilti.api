using System.Reflection;
using Application.Features.Orders.Consumers;
using Application.Features.Security.Extensions;
using Application.Features.Users.Consumers;
using Application.Shared.Abstractions;
using Domain.Features.Users.Events;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scrutor;

namespace Application.Configurations;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, WebApplicationBuilder builder)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // Register Mediator
        services.AddScoped<Application.Shared.Abstractions.IMediator, Application.Shared.Abstractions.Mediator>();

        // Register all handlers
        services.Scan(scan => scan
            .FromAssemblies(assembly)
            .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<,>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.Scan(scan => scan
            .FromAssemblies(assembly)
            .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.AddFluentValidationAutoValidation();
        services.AddFluentValidationClientsideAdapters();
        services.AddValidatorsFromAssembly(assembly);
        services.AddScoped<ISecurityExtensions, SecurityExtensions>();

        services.AddHostedService<UserCreatedConsumer>();
        services.AddHostedService<CloseExpiredOrdersConsumer>();

        return services;
    }
}
