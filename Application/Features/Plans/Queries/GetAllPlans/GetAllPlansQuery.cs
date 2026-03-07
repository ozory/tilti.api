using System.Collections.Immutable;
using Application.Features.Plans.Contracts;
using Application.Shared.Abstractions;
using FluentResults;

namespace Application.Features.Plans.Queries.GetAllPlans;

public class GetAllPlansQuery : IQuery<Result<ImmutableList<PlanResponse>>>
{
}
