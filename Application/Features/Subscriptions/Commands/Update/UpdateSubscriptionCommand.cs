using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Subscriptions.Contracts;
using Application.Shared.Abstractions;
using Domain.Subscriptions.Enums;

namespace Application.Features.Subscriptions.Commands.Update;

public sealed record UpdateSubscriptionCommand
(
    long SubscriptionId,
    long PlanId,
    DateTime DueDate,
    ushort Status
) : ICommand<SubscriptionResponse>;
