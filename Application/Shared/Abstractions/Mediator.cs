using System.Reflection;
using Application.Shared.Abstractions;
using Domain.Shared.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Shared.Abstractions;

public class Mediator : IMediator
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public Mediator(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task<TResponse> Send<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default)
    {
        using var scope = _serviceScopeFactory.CreateScope();

        // Get all implemented interfaces of the command type
        var commandType = command.GetType();
        var commandInterface = commandType.GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommand<>))
            ?? throw new InvalidOperationException($"Command type {commandType} does not implement ICommand<TResponse>");

        var handlerType = typeof(ICommandHandler<,>).MakeGenericType(commandType, commandInterface.GetGenericArguments()[0]);
        var handler = scope.ServiceProvider.GetRequiredService(handlerType);
        var method = handlerType.GetMethod("Handle")
            ?? throw new InvalidOperationException("Handle method not found on command handler");

        var result = await (Task<TResponse>)method.Invoke(handler, new object[] { command, cancellationToken })!;
        return result;
    }

    public async Task<TResponse> Send<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResponse));
        var handler = scope.ServiceProvider.GetRequiredService(handlerType);
        var method = handlerType.GetMethod("Handle")
            ?? throw new InvalidOperationException("Handle method not found on query handler");

        var result = await (Task<TResponse>)method.Invoke(handler, new object[] { query, cancellationToken })!;
        return result;
    }

    public async Task Publish(INotification notification, CancellationToken cancellationToken = default)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var handlerType = typeof(INotificationHandler<>).MakeGenericType(notification.GetType());
        var handlers = scope.ServiceProvider.GetServices(handlerType);

        var method = handlerType.GetMethod("Handle")
            ?? throw new InvalidOperationException("Handle method not found on notification handler");

        var tasks = handlers.Select(handler =>
            (Task)method.Invoke(handler, new object[] { notification, cancellationToken })!);

        await Task.WhenAll(tasks);
    }

    public async Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification
    {
        await Publish(notification, cancellationToken);
    }
}