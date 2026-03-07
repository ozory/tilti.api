using Application.Features.Orders.Contracts;
using Application.Shared.Abstractions;
using Domain.ValueObjects;
using FluentResults;
namespace Application.Features.Orders.Commands.AddMessage;

public sealed record AddMessageCommand
(
    long SourceUserId,
    long TargetUserId,
    long OrderId,
    string Message

) : ICommand<Result<bool>>;


