using Application.Shared.Abstractions;
using Domain.Features.Payments.Enums;
using Domain.Shared.Abstractions;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace Application.Features.Payments.Commands.UpdatePaymentStatus;

/// <summary>
/// Handler for updating payment status from webhook
/// </summary>
public class UpdatePaymentStatusCommandHandler : ICommandHandler<UpdatePaymentStatusCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdatePaymentStatusCommandHandler> _logger;
    private readonly string _className = nameof(UpdatePaymentStatusCommandHandler);

    public UpdatePaymentStatusCommandHandler(
        ILogger<UpdatePaymentStatusCommandHandler> logger,
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Handles the update of payment status
    /// </summary>
    public async Task<Result<bool>> Handle(UpdatePaymentStatusCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("[{ClassName}] Updating payment status for {AsaasPaymentId} to {Status}",
            _className, request.AsaasPaymentId, request.Status);

        try
        {
            // Get payment by Asaas ID
            var payment = await _unitOfWork.PaymentRepository.GetByAsaasPaymentId(request.AsaasPaymentId);
            if (payment == null)
            {
                _logger.LogWarning("[{ClassName}] Payment not found for AsaasPaymentId: {AsaasPaymentId}",
                    _className, request.AsaasPaymentId);
                return Result.Fail("Payment not found");
            }

            // Update status based on webhook status
            if (request.Status == "CONFIRMED" || request.Status == "RECEIVED")
            {
                payment.ApprovePayment();
            }
            else if (request.Status == "CANCELED" || request.Status == "REFUNDED")
            {
                payment.CancelPayment();
            }

            await _unitOfWork.PaymentRepository.UpdateAsync(payment);
            await _unitOfWork.CommitAsync(cancellationToken);

            _logger.LogInformation("[{ClassName}] Payment status updated for {AsaasPaymentId}",
                _className, request.AsaasPaymentId);

            return Result.Ok(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[{ClassName}] Error updating payment status for {AsaasPaymentId}",
                _className, request.AsaasPaymentId);
            return Result.Fail($"Error updating payment status: {ex.Message}");
        }
    }
}