using Microsoft.AspNetCore.Mvc;
using MediatR;
using Application.Features.Users.Commands.CreateUser;
using Application.Features.Users.Commands.RefreshToken;

namespace Api.Endpoints;

public static class SecurityEndpoint
{
    public static void MapSecurityEndpoint(this WebApplication app)
    {
        RouteGroupBuilder security = app.MapGroup("/security")
            .WithTags("Security");
        security.MapPost("/auth", Authenticate)
            .AllowAnonymous()
            .WithOpenApi();

        security.MapPost("/refresh", RefreshToken)
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

    private static async Task<IResult> RefreshToken(
        [FromBody] RefreshTokenCommand refreshTokenCommand,
        [FromServices] IMediator mediator
    )
    {
        try
        {
            var result = await mediator.Send(refreshTokenCommand);
            if (result.IsFailed) return TypedResults.BadRequest(result.Errors);
            return TypedResults.Ok(result.Value);
        }
        catch (Exception)
        {
            throw;
        }

    }
}
