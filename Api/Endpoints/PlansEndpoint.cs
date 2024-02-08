using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Plans.Commands.Create;
using Application.Features.Plans.Commands.Update;
using Application.Features.Plans.Queries.GetAllPlans;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints;

public static class PlansEndpoint
{
    public static void MapPlansEndpoint(this WebApplication app)
    {
        RouteGroupBuilder users = app.MapGroup("/plans");
        users.MapGet("/", GetAllUsers).WithOpenApi();
        users.MapPost("/", CreatePlan).WithOpenApi();
        users.MapPut("/", UpdatePlan).WithOpenApi();
    }

    private static async Task<IResult> GetAllUsers(
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
