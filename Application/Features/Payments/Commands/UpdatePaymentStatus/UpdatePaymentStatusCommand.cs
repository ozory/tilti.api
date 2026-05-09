using Application.Shared.Abstractions;
using FluentResults;

namespace Application.Features.Payments.Commands.UpdatePaymentStatus;

/// <summary>
/// Command to update payment status when webhook is received
/// </summary>
/// <param name="AsaasPaymentId">Asaas payment ID</param>
/// <param name="Status">New status (CONFIRMED, RECEIVED, etc.)</param>
public record UpdatePaymentStatusCommand(
    string AsaasPaymentId,
    string Status
) : ICommand<Result<bool>>;