using Microsoft.AspNetCore.Mvc;
using MediatR;
using Application.Features.User.Commands.CreateUser;

namespace Api.Endpoints;

public static class SecurityEndpoint
{
    public static void MapSecurityEndpoint(this WebApplication app)
    {
        RouteGroupBuilder users = app.MapGroup("/security");
        users.MapPost("/auth", Authenticate)
            .AllowAnonymous()
            .WithOpenApi();
    }

    private static async Task<IResult> Authenticate(
        [FromBody] AuthenticateUserCommand authenticateUserCommand,
        [FromServices] IMediator mediator
    )
    {
        try
        {
            var result = await mediator.Send(authenticateUserCommand);
            if (result.IsFailed) return TypedResults.BadRequest(result.Errors);
            return TypedResults.Ok(result.Value);
        }
        catch (Exception)
        {
            throw;
        }

    }
}
