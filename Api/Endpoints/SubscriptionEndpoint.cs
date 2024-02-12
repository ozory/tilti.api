using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Subscriptions.Commands.Create;
using Application.Features.Subscriptions.Commands.Update;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints
{
    public static class SubscriptionEndpoint
    {
        public static void MapSubscriptionsEndpoint(this WebApplication app)
        {
            RouteGroupBuilder subs = app.MapGroup("/subscriptions")
                .WithTags("Subscriptions");

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
    }
}