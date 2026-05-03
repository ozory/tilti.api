using Application.Features.Payments.Contracts;
using Application.Shared.Abstractions;
using FluentResults;

namespace Application.Features.Payments.Commands.CreatePassengerPayment;

/// <summary>
/// Command to create a passenger payment for a ride
/// </summary>
/// <param name="UserId">Passenger user ID</param>
/// <param name="Amount">Payment amount</param>
/// <param name="ExternalReference">External reference (Order ID)</param>
/// <param name="DueDate">Payment due date (15-30 min from now)</param>
public record CreatePassengerPaymentCommand(
    long UserId,
    decimal Amount,
    string ExternalReference,
    DateTime DueDate
) : ICommand<Result<PaymentResponse>>;
