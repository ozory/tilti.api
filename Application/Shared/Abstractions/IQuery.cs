using FluentResults;
using MediatR;

namespace Application.Shared.Abstractions
{
    public interface IQuery<TResponse> : IRequest<Result<TResponse>>
    { }
}