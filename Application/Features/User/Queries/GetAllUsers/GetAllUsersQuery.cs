using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.User.Contracts;
using FluentResults;
using MediatR;

namespace Application.Features.User.Queries.GetAllUsers;

public class GetAllUsersQuery : IRequest<Result<ImmutableList<UserResponse>>>
{

}
