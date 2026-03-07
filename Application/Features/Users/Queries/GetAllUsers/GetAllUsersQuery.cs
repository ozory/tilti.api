using System.Collections.Immutable;
using Application.Features.Users.Contracts;
using Application.Shared.Abstractions;
using FluentResults;

namespace Application.Features.Users.Queries.GetAllUsers;

public class GetAllUsersQuery : IQuery<Result<ImmutableList<UserResponse>>>
{
}
