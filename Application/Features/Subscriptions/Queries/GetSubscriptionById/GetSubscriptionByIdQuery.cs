using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Subscriptions.Contracts;
using Application.Shared.Abstractions;
using FluentResults;

namespace Application.Features.Subscriptions.Queries.GetMemberById;

public sealed record GetSubscriptionByIdQuery
(
    long subscriptionId
) : IQuery<SubscriptionResponse>;
