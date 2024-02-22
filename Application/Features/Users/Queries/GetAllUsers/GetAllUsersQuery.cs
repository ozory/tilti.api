using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Users.Contracts;
using FluentResults;
using MediatR;

namespace Application.Features.Users.Queries.GetAllUsers;

public class GetAllUsersQuery : IRequest<Result<ImmutableList<UserResponse>>>
{

}
