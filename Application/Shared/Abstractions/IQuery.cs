using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentResults;
using MediatR;

namespace Application.Shared.Abstractions
{
    public interface IQuery<TResponse> : IRequest<Result<TResponse>>
    { }
}