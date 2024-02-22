using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Application.Features.Orders.Services;
using Application.Features.Security.Extensions;
using Application.Features.Users.Commands.CreateUser;
using Application.Shared.Abstractions;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DomainUser = Domain.Features.Users.Entities.User;
namespace Application.Configurations;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        var assembly = Assembly.GetExecutingAssembly();
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(assembly);
        });

        services.AddFluentValidationAutoValidation();
        services.AddFluentValidationClientsideAdapters();
        services.AddValidatorsFromAssembly(assembly);

        services.AddScoped<ISecurityExtensions, SecurityExtensions>();

        var valuePerKM = Decimal.Parse(
            configuration.GetSection("Configurations:PricePerKM").Value!,
            CultureInfo.InvariantCulture);

        var valuePerDuration = Decimal.Parse(
            configuration.GetSection("Configurations:PricePerDuration").Value!,
            CultureInfo.InvariantCulture);

        var apiKey = configuration.GetSection("Configurations:ApiKey").Value;

        services.AddSingleton<IMapServices>(
            new MapServices(valuePerKM, valuePerDuration, apiKey!));

        return services;
    }
}
