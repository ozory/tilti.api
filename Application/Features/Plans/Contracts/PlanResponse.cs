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
    public static implicit operator PlanResponse(Plan plan)
        => new PlanResponse(
                plan.Id,
                plan.Name.Value!,
                plan.Description.Value!,
                plan.Amount.Value,
                plan.Status.ToString(),
                plan.CreatedAt,
                plan.UpdatedAt);
}
