using Application.Features.Orders.Commands.AddMessage;
using Application.Features.Orders.Commands.CreateOrder;
using Application.Features.Orders.Commands.PrecifyOrder;
using Application.Features.Orders.Queries.GetOrdersByPoint;
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
        orders.MapPost("/getByPoint", GetByPoint).WithOpenApi();
        orders.MapPost("/message", AddMessage).WithOpenApi();
    }

    private static async Task<IResult> GetByPoint(
           [FromQuery(Name = "lat")] double lat,
           [FromQuery(Name = "lnt")] double lnt,
           [FromQuery(Name = "driverId")] long? driverId,
           [FromServices] IMediator mediator)
    {
        try
        {
            var result = await mediator.Send(new GetAllOrdersByPointQuery
            {
                Latitude = lat,
                Longitude = lnt,
                DriverId = driverId
            });
            if (result.IsFailed) return TypedResults.BadRequest(result.Errors);
            return TypedResults.Ok(result.Value);
        }
        catch (Exception)
        {
            throw;
        }
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

    private static async Task<IResult> AddMessage(
     [FromBody] AddMessageCommand addMessageCommand,
     [FromServices] IMediator mediator)
    {
        try
        {
            var result = await mediator.Send(addMessageCommand);
            if (result.IsFailed) return TypedResults.BadRequest(result.Errors);
            return TypedResults.Ok(result.Value);
        }
        catch (Exception)
        {
            throw;
        }
    }
}
