using Application.Features.Subscriptions.Contracts;
using Application.Shared.Abstractions;
using Domain.Features.Subscriptions.Repository;
using Domain.Features.Users.Repository;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace Application.Features.Subscriptions.Queries.GetSubscriptionByUser;

public class GetSubscriptionByUserQueryHandler : IQueryHandler<GetSubscriptionByUserQuery, SubscriptionResponse>
{
    private readonly ISubscriptionRepository _repository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetSubscriptionByUserQueryHandler> _logger;
    private readonly string className = nameof(GetSubscriptionByUserQueryHandler);

    public GetSubscriptionByUserQueryHandler(
        ISubscriptionRepository repository,
        IUserRepository userRepository,
        ILogger<GetSubscriptionByUserQueryHandler> logger)
    {
        _repository = repository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<Result<SubscriptionResponse>> Handle(
        GetSubscriptionByUserQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("[{className}] Getting subscription for user {UserId}", className, request.userId);
        try
        {
            var user = await _userRepository.GetByIdAsync(request.userId);
            if (user == null) return Result.Fail("User not found");

            var subscription = await _repository.GetSubscriptionByUser(request.userId);
            if (subscription == null) return Result.Fail("User has no active subscription");

            return Result.Ok((SubscriptionResponse)subscription);
        }
        catch (Exception ex)
        {
            _logger.LogError("[{className}] Error getting subscription for user {UserId}: {Error}",
                className, request.userId, ex);
            throw;
        }
    }
}