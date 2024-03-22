using Application.Features.Subscriptions.Contracts;
using Application.Shared.Abstractions;

namespace Application.Features.Subscriptions.Commands.Create;

public sealed record CreateSubscriptionCommand
(
    long userId,
    long planId
) : ICommand<SubscriptionResponse>;
