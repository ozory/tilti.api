using FluentResults;
using MediatR;

namespace Application.Shared.Abstractions;

public interface ICommand : IRequest<Result> { }

public interface ICommand<TResponse> : IRequest<Result<TResponse>> { }
