using Application.Features.Subscriptions.Commands.Create;
using Application.Features.Subscriptions.Commands.Update;
using Application.Features.Subscriptions.Queries.GetAllSubscriptions;
using Application.Features.Subscriptions.Queries.GetMemberById;
using Application.Features.Subscriptions.Queries.GetSubscriptionByUser;
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
            subs.MapPost("/", CreateSubscription).WithOpenApi();
            subs.MapPut("/", UpdateSubscription).WithOpenApi();
        }

        private static async Task<IResult> CreateSubscription(
        [FromBody] CreateSubscriptionCommand createSubscriptionCommand,
        [FromServices] IMediator mediator)
        {
            try
            {
                var result = await mediator.Send(createSubscriptionCommand);
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