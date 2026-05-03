using Application.Features.Payments.Commands.CreatePassengerPayment;
using Application.Shared.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints;

/// <summary>
/// Endpoints for passenger payment operations
/// </summary>
public static class PaymentsEndpoint
{
    /// <summary>
    /// Maps payment endpoints to the web application
    /// </summary>
    public static void MapPaymentsEndpoint(this WebApplication app)
    {
        var payments = app.MapGroup("/payments")
            .WithTags("Payments");

        payments.MapPost("/passenger/create", CreatePassengerPayment).WithOpenApi();
    }

    /// <summary>
    /// Create a payment for passenger ride
    /// </summary>
    private static async Task<IResult> CreatePassengerPayment(
        [FromBody] CreatePassengerPaymentCommand command,
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
}
