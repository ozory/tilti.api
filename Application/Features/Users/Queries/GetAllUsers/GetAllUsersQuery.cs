using System.Collections.Immutable;
using Application.Features.Users.Contracts;
using FluentResults;
using MediatR;

namespace Application.Features.Users.Queries.GetAllUsers;

public class GetAllUsersQuery : IRequest<Result<ImmutableList<UserResponse>>>
{

}
