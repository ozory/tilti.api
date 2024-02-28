using System.Collections.Immutable;
using Application.Features.Plans.Contracts;
using FluentResults;
using MediatR;

namespace Application.Features.Plans.Queries.GetAllPlans;

public class GetAllPlansQuery : IRequest<Result<ImmutableList<PlanResponse>>>
{

}
