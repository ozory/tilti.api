using System.Reflection;
using Application.Features.Security.Extensions;
using Application.Shared.Abstractions;
using FluentValidation;
using FluentValidation.AspNetCore;
using MassTransit;
using MediatR.NotificationPublishers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace Application.Configurations;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        var assembly = Assembly.GetExecutingAssembly();
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(assembly);
            config.NotificationPublisher = new TaskWhenAllPublisher();
        });

        services.AddFluentValidationAutoValidation();
        services.AddFluentValidationClientsideAdapters();
        services.AddValidatorsFromAssembly(assembly);

        services.AddMassTransit(busConfig =>
        {
            busConfig.SetKebabCaseEndpointNameFormatter();

            busConfig.AddConsumers(assembly);

            busConfig.UsingRabbitMq((ctx, config) =>
            {
                config.Host(configuration["Infrastructure:RabbitMQ:Server"]!, "/",
                    h =>
                        {
                            h.Username(configuration["Infrastructure:RabbitMQ:Username"]!);
                            h.Password(configuration["Infrastructure:RabbitMQ:Password"]!);
                        });
                config.ConfigureEndpoints(ctx);
            });

        });

        services.AddScoped<ISecurityExtensions, SecurityExtensions>();
        return services;
    }
}
