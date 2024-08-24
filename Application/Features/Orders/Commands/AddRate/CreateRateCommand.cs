
using Application.Shared.Abstractions;
namespace Application.Features.Orders.Commands.AddRate;

public sealed record CreateRateCommand
(
    long OrderId,
    long SourceUserId,
    long TargetUserId,
    float Value,
    string? Description = null,
    string? Tags = null

) : ICommand<bool>;


