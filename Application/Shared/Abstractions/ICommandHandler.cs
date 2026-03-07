using FluentResults;

namespace Application.Shared.Abstractions;

public interface ICommandHandler<TCommand, TResponse> where TCommand : ICommand<Result<TResponse>>
{
    Task<Result<TResponse>> Handle(TCommand command, CancellationToken cancellationToken);
}

public interface ICommandHandler<TCommand> where TCommand : ICommand<Result>
{
    Task<Result> Handle(TCommand command, CancellationToken cancellationToken);
}