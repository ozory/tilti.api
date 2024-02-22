using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Users.Commands.CreateUser;
using Application.Features.Users.Contracts;
using Domain.Features.Users.Repository;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Users.Queries.GetAllUsers;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, Result<ImmutableList<UserResponse>>>
{
    private readonly IUserRepository _repository;
    private readonly ILogger<GetAllUsersQueryHandler> _logger;

    public GetAllUsersQueryHandler(
        IUserRepository repository,
        ILogger<GetAllUsersQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<ImmutableList<UserResponse>>> Handle(
        GetAllUsersQuery request,
        CancellationToken cancellationToken)
    {
        var result = await _repository.GetAllAsync();
        return Result.Ok(result.Select(x => (UserResponse)x).ToImmutableList());
    }
}
