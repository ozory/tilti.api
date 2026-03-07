using Application.Features.Plans.Contracts;
using Application.Shared.Abstractions;
using FluentResults;

namespace Application.Features.Plans.Queries.GetPlanBy;

public class GetPlanByIdQuery : IQuery<Result<PlanResponse?>>
{
    public long Id { get; set; }
}
