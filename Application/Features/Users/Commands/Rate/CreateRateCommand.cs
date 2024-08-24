using Application.Features.Users.Contracts;
using Application.Shared.Abstractions;
namespace Application.Features.Users.Commands.Rate;

public sealed record CreateRateCommand
(
    long orderId,
    long sourceUserId,
    long targetUserId,
    float value

) : ICommand<bool>;


