using Application.Features.Subscriptions.Commands.CreateDriverSubscription;
using Application.Features.Subscriptions.Commands.ActivateDriverSubscription;
using Application.Features.Subscriptions.Commands.CancelDriverSubscription;
using Application.Features.Subscriptions.Commands.Update;
using Application.Features.Subscriptions.Queries.GetAllSubscriptions;
using Application.Features.Subscriptions.Queries.GetMemberById;
using Application.Features.Subscriptions.Queries.GetSubscriptionByUser;
using Application.Features.Subscriptions.Queries.GetDriverSubscription;
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

            // Driver subscription endpoints
            subs.MapPost("/driver", CreateDriverSubscription).WithOpenApi();
            subs.MapGet("/driver/{userId}", GetDriverSubscription).WithOpenApi();
            subs.MapPost("/driver/activate", ActivateDriverSubscription).WithOpenApi();
            subs.MapPost("/driver/cancel", CancelDriverSubscription).WithOpenApi();
        }

        private static async Task<IResult> CreateDriverSubscription(
        [FromBody] CreateDriverSubscriptionCommand command,
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

        private static async Task<IResult> GetDriverSubscription(
        [FromRoute] long userId,
        [FromServices] IMediator mediator)
        {
            try
            {
                var result = await mediator.Send(new GetDriverSubscriptionQuery(userId));
                if (result.IsFailed) return TypedResults.BadRequest(result.Errors);
                return TypedResults.Ok(result.Value);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static async Task<IResult> ActivateDriverSubscription(
        [FromBody] ActivateDriverSubscriptionCommand command,
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

        private static async Task<IResult> CancelDriverSubscription(
        [FromBody] CancelDriverSubscriptionCommand command,
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