using FluentResults;

namespace Application.Shared.Abstractions;

public interface IQueryHandler<TQuery, TResponse> where TQuery : IQuery<Result<TResponse>>
{
    Task<Result<TResponse>> Handle(TQuery query, CancellationToken cancellationToken);
}