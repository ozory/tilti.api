using Application.Features.Plans.Contracts;
using Application.Shared.Abstractions;
using Domain.Features.Plans.Repository;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace Application.Features.Plans.Queries.GetPlanBy;

public class GetPlanByIdQueryHandler : IQueryHandler<GetPlanByIdQuery, PlanResponse?>
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
