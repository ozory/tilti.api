using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Features.Subscriptions.Contracts;

public sealed record SubscriptionResponse
(
    long Id,
    string PlanName,
    decimal Value,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    DateTime DueDate
);
