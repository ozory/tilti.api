using System.Collections.Immutable;
using Application.Features.Plans.Contracts;
using Domain.Features.Plans.Entities;
using Domain.Features.Plans.Repository;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Plans.Queries.GetPlanBy;

public class GetPlanByIdQueryHandler : IRequestHandler<GetPlanByIdQuery, Result<PlanResponse?>>
{
    private readonly IPlanRepository _repository;
    private readonly ILogger<GetPlanByIdQueryHandler> _logger;

    public GetPlanByIdQueryHandler(
        IPlanRepository repository,
        ILogger<GetPlanByIdQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<PlanResponse?>> Handle(
        GetPlanByIdQuery request,
        CancellationToken cancellationToken)
    {
        var result = await _repository.GetByIdAsync(request.Id);
        return Result.Ok((PlanResponse?)result);
    }
}
