using Application.Features.Orders.Commands.AddMessage;
using Application.Features.Orders.Commands.AddTracking;
using Application.Features.Orders.Commands.CancelOrder;
using Application.Features.Orders.Commands.CreateOrder;
using Application.Features.Orders.Commands.FinishOrder;
using Application.Features.Orders.Commands.PrecifyOrder;
using Application.Features.Orders.Queries.GetOrdersByPoint;
using Application.Features.Orders.Queries.GetOrdersByUser;
using Application.Shared.Abstractions;
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
        orders.MapPost("/tracking", AddTracking).WithOpenApi();
        orders.MapPost("/{orderId}/finish", FinishOrder).WithOpenApi();
        orders.MapPost("/{orderId}/cancel", CancelOrder).WithOpenApi();
        orders.MapGet("/user/{userId}", GetOrdersByUser).WithOpenApi();
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

    private static async Task<IResult> GetOrdersByUser(
        [FromRoute] long userId,
        [FromServices] IMediator mediator)
    {
        try
        {
            var result = await mediator.Send(new GetOrdersByUserQuery(userId));
            if (result.IsFailed) return TypedResults.BadRequest(result.Errors);
            return TypedResults.Ok(result.Value);
        }
        catch (Exception)
        {
            throw;
        }
    }

    private static async Task<IResult> CancelOrder(
        [FromRoute] long orderId,
        [FromBody] CancelOrderCommand cancelOrderCommand,
        [FromServices] IMediator mediator)
    {
        try
        {
            var command = cancelOrderCommand with { OrderId = orderId };
            var result = await mediator.Send(command);
            if (result.IsFailed) return TypedResults.BadRequest(result.Errors);
            return TypedResults.Ok(result.Value);
        }
        catch (Exception)
        {
            throw;
        }
    }

    private static async Task<IResult> AddTracking(
        [FromBody] AddTrackingCommand command,
        [FromServices] IMediator mediator)
    {
        try
        {
            var result = await mediator.Send(command);
            if (result.IsFailed) return TypedResults.BadRequest(result.Errors);
            return TypedResults.Ok(result.Value);
        }
        catch (Exception)
        {
            throw;
        }
    }

    private static async Task<IResult> FinishOrder(
        [FromRoute] long orderId,
        [FromBody] FinishOrderCommand command,
        [FromServices] IMediator mediator)
    {
        try
        {
            var finishCommand = new FinishOrderCommand(orderId, command.UserId);
            var result = await mediator.Send(finishCommand);
            if (result.IsFailed) return TypedResults.BadRequest(result.Errors);
            return TypedResults.Ok(result.Value);
        }
        catch (Exception)
        {
            throw;
        }
    }
}
