using Application.Features.Plans.Commands.Create;
using Application.Features.Plans.Commands.Update;
using Application.Features.Plans.Queries.GetAllPlans;
using Application.Features.Plans.Queries.GetPlanBy;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints;

public static class PlansEndpoint
{
    public static void MapPlansEndpoint(this WebApplication app)
    {
        RouteGroupBuilder users = app.MapGroup("/plans")
            .WithTags("Plans");
        users.MapGet("/", GetAllPlans).WithOpenApi();
        users.MapGet("/{id}", GetById).WithOpenApi();
        users.MapPost("/", CreatePlan).WithOpenApi();
        users.MapPut("/", UpdatePlan).WithOpenApi();
    }

    private static async Task<IResult> GetAllPlans(
      [FromServices] IMediator mediator
    )
    {
        try
        {
            var result = await mediator.Send(new GetAllPlansQuery());
            if (result.IsFailed) return TypedResults.BadRequest(result.Errors);
            return TypedResults.Ok(result.Value);
        }
        catch (Exception)
        {
            throw;
        }

    }

    private static async Task<IResult> GetById(
        [FromRoute] long id,
        [FromServices] IMediator mediator
    )
    {
        try
        {
            var result = await mediator.Send(new GetPlanByIdQuery() { Id = id });
            if (result.IsFailed) return TypedResults.BadRequest(result.Errors);
            return TypedResults.Ok(result.Value);
        }
        catch (Exception)
        {
            throw;
        }

    }

    private static async Task<IResult> CreatePlan(
        [FromBody] CreatePlanCommand createPlanCommand,
        [FromServices] IMediator mediator
    )
    {
        try
        {
            var result = await mediator.Send(createPlanCommand);
            if (result.IsFailed) return TypedResults.BadRequest(result.Errors);
            return TypedResults.Ok(result.Value);
        }
        catch (Exception)
        {
            throw;
        }

    }

    private static async Task<IResult> UpdatePlan(
        [FromBody] UpdatePlanCommand updatePlanCommand,
        [FromServices] IMediator mediator
    )
    {
        try
        {
            var result = await mediator.Send(updatePlanCommand);
            if (result.IsFailed) return TypedResults.BadRequest(result.Errors);
            return TypedResults.Ok(result.Value);
        }
        catch (Exception)
        {
            throw;
        }

    }
}
