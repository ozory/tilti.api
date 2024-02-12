using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Subscriptions.Commands.Create;
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
            // subs.MapGet("/", GetAllUsers).WithOpenApi();
            subs.MapPost("/", CreatePlan).WithOpenApi();
            // subs.MapPut("/", UpdatePlan).WithOpenApi();
        }

        private static async Task<IResult> CreatePlan(
        [FromBody] CreateSubscriptionCommand createSubscriptionCommand,
        [FromServices] IMediator mediator
    )
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
    }
}