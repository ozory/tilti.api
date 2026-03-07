using Application.Features.Plans.Contracts;
using Application.Shared.Abstractions;
using FluentResults;

namespace Application.Features.Plans.Commands.Update;

public record UpdatePlanCommand
(
    long Id,
    string Name,
    string Description,
    decimal Amount,
    ushort Status = 0
) : ICommand<Result<PlanResponse>>;