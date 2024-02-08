using Application.Features.Plans.Contracts;
using Application.Shared.Abstractions;

namespace Application.Features.Plans.Commands.Update;

public record UpdatePlanCommand
(
    long Id,
    string Name,
    string Description,
    decimal Amount,
    ushort Status
) : ICommand<PlanResponse>;