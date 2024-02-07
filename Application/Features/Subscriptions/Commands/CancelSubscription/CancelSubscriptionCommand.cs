using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Shared.Abstractions;
using Domain.Subscriptions.Enums;

namespace Application.Features.Subscriptions.Commands.CancelSubscription;

public sealed record CancelSubscriptionCommand
(
    long subscriptionId
) : ICommand;
