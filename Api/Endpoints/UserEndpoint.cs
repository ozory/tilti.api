using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Application.Features.User.Commands.CreateUser;
using Application.Features.User.Commands.UpdateUser;
using Application.Features.User.Queries.GetAllUsers;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace Api.Endpoints;

public static class UserEndpoint
{
    public static void MapUsersEndpoint(this WebApplication app)
    {
        RouteGroupBuilder users = app.MapGroup("/users")
            .RequireAuthorization("multi")
            .WithTags("Users");
        users.MapGet("/", GetAllUsers).WithOpenApi();
        users.MapPost("/", CreateUser).WithOpenApi();
        users.MapPut("/", UpdateUser).WithOpenApi();
    }

    private static async Task<IResult> GetAllUsers(
        [FromServices] IMediator mediator,
        ClaimsPrincipal user
    )
    {
        try
        {
            var result = await mediator.Send(new GetAllUsersQuery());
            if (result.IsFailed) return TypedResults.BadRequest(result.Errors);
            return TypedResults.Ok(result.Value);
        }
        catch (Exception)
        {
            throw;
        }

    }

    private static async Task<IResult> CreateUser(
        [FromBody] CreateUserCommand createUserCommand,
        [FromServices] IMediator mediator
    )
    {
        try
        {
            var result = await mediator.Send(createUserCommand);
            if (result.IsFailed) return TypedResults.BadRequest(result.Errors);
            return TypedResults.Ok(result.Value);
        }
        catch (Exception)
        {
            throw;
        }

    }

    private static async Task<IResult> UpdateUser(
        [FromBody] UpdateUserCommand updateUserCommand,
        [FromServices] IMediator mediator,
        ClaimsPrincipal user
    )
    {
        try
        {
            // var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            // string? id = jwt.Claims.FirstOrDefault(c => c.Type == "nameid")?.Value;

            // if (id is null || id != updateUserCommand.id.ToString()) return TypedResults.Forbid();

            var result = await mediator.Send(updateUserCommand);
            if (result.IsFailed) return TypedResults.BadRequest(result.Errors);
            return TypedResults.Ok(result.Value);
        }
        catch (Exception)
        {
            throw;
        }

    }
}
