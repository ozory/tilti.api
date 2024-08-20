using Application.Features.Users.Contracts;
using Application.Shared.Abstractions;
namespace Application.Features.Users.Commands.CreateUser;

public sealed record CreateRateCommand
(
    long sourceUserId,
    long targetUserId,
    float value

) : ICommand<bool>;


