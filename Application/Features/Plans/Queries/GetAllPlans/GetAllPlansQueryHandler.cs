using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Plans.Contracts;
using Application.Features.User.Contracts;
using Domain.Features.Plans.Repository;
using Domain.Features.Users.Repository;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Plans.Queries.GetAllPlans;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllPlansQuery, Result<ImmutableList<PlanResponse>>>
{
    private readonly IPlanRepository _repository;
    private readonly ILogger<GetAllUsersQueryHandler> _logger;

    public GetAllUsersQueryHandler(
        IPlanRepository repository,
        ILogger<GetAllUsersQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<ImmutableList<PlanResponse>>> Handle(
        GetAllPlansQuery request,
        CancellationToken cancellationToken)
    {
        var result = await _repository.GetAllAsync();
        return Result.Ok(result.Select(x => (PlanResponse)x).ToImmutableList());
    }
}
