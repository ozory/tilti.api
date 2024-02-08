using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Plans.Entities;

namespace Application.Features.Plans.Contracts;

public record PlanResponse
(
    long? Id,
    string Name,
    string Description,
    decimal Amount,
    string Status,
    DateTime? CreatedAt,
    DateTime? UpdatedAt
)
{
    public static implicit operator PlanResponse(Plan user)
        => new PlanResponse(
                user.Id,
                user.Name.Value!,
                user.Description.Value!,
                user.Amount.Value,
                user.Status.ToString(),
                user.CreatedAt,
                user.UpdatedAt);
}
