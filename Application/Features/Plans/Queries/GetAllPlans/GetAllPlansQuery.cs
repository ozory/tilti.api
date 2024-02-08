using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Plans.Contracts;
using FluentResults;
using MediatR;

namespace Application.Features.Plans.Queries.GetAllPlans;

public class GetAllPlansQuery : IRequest<Result<ImmutableList<PlanResponse>>>
{

}
