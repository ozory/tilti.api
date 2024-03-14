using System.Reflection;
using Application.Features.Security.Extensions;
using Application.Features.Users.Consumers;
using Application.Shared.Abstractions;
using Domain.Features.Users.Events;
using FluentValidation;
using FluentValidation.AspNetCore;
using MassTransit;
using MediatR.NotificationPublishers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wolverine;
using Wolverine.RabbitMQ;
namespace Application.Configurations;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, WebApplicationBuilder builder)
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
        services.AddScoped<ISecurityExtensions, SecurityExtensions>();

        builder.Host.UseWolverine(opts =>
        {
            opts.UseRabbitMq(rabbit =>
            {
                rabbit.HostName = builder.Configuration["Infrastructure:RabbitMQ:Server"];
                rabbit.UserName = builder.Configuration["Infrastructure:RabbitMQ:Username"];
                rabbit.Password = builder.Configuration["Infrastructure:RabbitMQ:Password"];
            })
            .AddUserMessageDefinitions(builder, opts);



        });

        return services;
    }
}
