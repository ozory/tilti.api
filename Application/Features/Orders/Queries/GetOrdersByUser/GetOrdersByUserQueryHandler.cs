using System.Collections.Immutable;
using Application.Features.Orders.Contracts;
using Application.Shared.Abstractions;
using Domain.Features.Orders.Repository;
using Domain.Features.Users.Repository;
using FluentResults;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Application.Features.Orders.Queries.GetOrdersByUser;

public class GetOrdersByUserQueryHandler : IQueryHandler<GetOrdersByUserQuery, ImmutableList<OrderResponse>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetOrdersByUserQueryHandler> _logger;
    private readonly string className = nameof(GetOrdersByUserQueryHandler);

    public GetOrdersByUserQueryHandler(
        IOrderRepository orderRepository,
        IUserRepository userRepository,
        ILogger<GetOrdersByUserQueryHandler> logger)
    {
        _orderRepository = orderRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<Result<ImmutableList<OrderResponse>>> Handle(GetOrdersByUserQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("[{className}] Getting orders for user {UserId}", className, request.UserId);
        try
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null) return Result.Fail("User not found");

            var orders = await _orderRepository.GetOrdersByUser(request.UserId);
            if (orders == null || !orders.Any())
                return Result.Ok(ImmutableList<OrderResponse>.Empty);

            return Result.Ok(orders.Select(x => (OrderResponse)x!).ToImmutableList());
        }
        catch (Exception ex)
        {
            _logger.LogError("[{className}] Error getting orders for user {UserId}: {Error}", className, request.UserId, ex);
            throw;
        }
    }
}