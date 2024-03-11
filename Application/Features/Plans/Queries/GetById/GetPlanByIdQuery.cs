using System.Collections.Immutable;
using Application.Features.Plans.Contracts;
using FluentResults;
using MediatR;

namespace Application.Features.Plans.Queries.GetPlanBy;

public class GetPlanByIdQuery : IRequest<Result<PlanResponse?>>
{
    public long Id { get; set; }
}
