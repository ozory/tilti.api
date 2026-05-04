using Application.Features.Subscriptions.Commands.CreateSubscription;
using Application.Features.Subscriptions.Commands.ActivateSubscription;
using Application.Features.Subscriptions.Commands.CancelSubscription;
using Application.Features.Subscriptions.Commands.Update;
using Application.Features.Subscriptions.Queries.GetAllSubscriptions;
using Application.Features.Subscriptions.Queries.GetMemberById;
using Application.Features.Subscriptions.Queries.GetSubscriptionByUser;
using Application.Features.Subscriptions.Queries.GetSubscription;
using Application.Shared.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints
{
    public static class SubscriptionEndpoint
    {
        public static void MapSubscriptionsEndpoint(this WebApplication app)
        {
            RouteGroupBuilder subs = app.MapGroup("/subscriptions")
                .WithTags("Subscriptions");

            subs.MapGet("/", GetAllSubscriptions).WithOpenApi();
            subs.MapGet("/{id}", GetSubscriptionById).WithOpenApi();
            subs.MapGet("/user/{userId}", GetSubscriptionByUser).WithOpenApi();
            subs.MapPut("/", UpdateSubscription).WithOpenApi();

            // Subscription endpoints (unified for Driver and Passenger)
            subs.MapPost("/create", CreateSubscription).WithOpenApi();
            subs.MapGet("/get/{userId}", GetSubscription).WithOpenApi();
            subs.MapPost("/activate", ActivateSubscription).WithOpenApi();
            subs.MapPost("/cancel", CancelSubscription).WithOpenApi();
        }

        private static async Task<IResult> CreateSubscription(
        [FromBody] CreateSubscriptionCommand command,
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

        private static async Task<IResult> GetSubscription(
        [FromRoute] long userId,
        [FromServices] IMediator mediator)
        {
            try
            {
                var result = await mediator.Send(new GetSubscriptionQuery(userId));
                if (result.IsFailed) return TypedResults.BadRequest(result.Errors);
                return TypedResults.Ok(result.Value);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static async Task<IResult> ActivateSubscription(
        [FromBody] ActivateSubscriptionCommand command,
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

        private static async Task<IResult> CancelSubscription(
        [FromBody] CancelSubscriptionCommand command,
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

        private static async Task<IResult> UpdateSubscription(
        [FromBody] UpdateSubscriptionCommand updateSubscriptionCommand,
        [FromServices] IMediator mediator)
        {
            try
            {
                var result = await mediator.Send(updateSubscriptionCommand);
                if (result.IsFailed) return TypedResults.BadRequest(result.Errors);
                return TypedResults.Ok(result.Value);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static async Task<IResult> GetAllSubscriptions(
            [FromServices] IMediator mediator)
        {
            try
            {
                var result = await mediator.Send(new GetAllSubscriptionsQuery());
                if (result.IsFailed) return TypedResults.BadRequest(result.Errors);
                return TypedResults.Ok(result.Value);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static async Task<IResult> GetSubscriptionById(
            [FromRoute] long id,
            [FromServices] IMediator mediator)
        {
            try
            {
                var result = await mediator.Send(new GetSubscriptionByIdQuery(id));
                if (result.IsFailed) return TypedResults.BadRequest(result.Errors);
                return TypedResults.Ok(result.Value);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static async Task<IResult> GetSubscriptionByUser(
            [FromRoute] long userId,
            [FromServices] IMediator mediator)
        {
            try
            {
                var result = await mediator.Send(new GetSubscriptionByUserQuery(userId));
                if (result.IsFailed) return TypedResults.BadRequest(result.Errors);
                return TypedResults.Ok(result.Value);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}