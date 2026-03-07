using Application.Features.Plans.Contracts;
using Application.Shared.Abstractions;
using FluentResults;

namespace Application.Features.Plans.Commands.Create;

public record CreatePlanCommand
(
    string Name,
    string Description,
    decimal Amount
) : ICommand<Result<PlanResponse>>;