using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Orders.Commands.CreateOrder;
using Application.Features.Orders.Commands.PrecifyOrder;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints;

public static class OrdersEndpoint
{
    public static void MapOrdersEndpoint(this WebApplication app)
    {
        RouteGroupBuilder orders = app.MapGroup("/orders").WithTags("Orders");
        orders.MapPost("/", CreateOrder).WithOpenApi();
        orders.MapPost("/precify", PrecifyOrder).WithOpenApi();
    }

    private static async Task<IResult> PrecifyOrder(
       [FromBody] PrecifyOrderCommand precifyOrderCommand,
       [FromServices] IMediator mediator)
    {
        try
        {
            var result = await mediator.Send(precifyOrderCommand);
            if (result.IsFailed) return TypedResults.BadRequest(result.Errors);
            return TypedResults.Ok(result.Value);
        }
        catch (Exception)
        {
            throw;
        }
    }

    private static async Task<IResult> CreateOrder(
        [FromBody] CreateOrderCommand createOrderCommand,
        [FromServices] IMediator mediator)
    {
        try
        {
            var result = await mediator.Send(createOrderCommand);
            if (result.IsFailed) return TypedResults.BadRequest(result.Errors);
            return TypedResults.Ok(result.Value);
        }
        catch (Exception)
        {
            throw;
        }
    }
}
