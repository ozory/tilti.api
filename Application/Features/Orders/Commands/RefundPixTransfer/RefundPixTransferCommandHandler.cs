using Application.Features.Orders.Events;
using Application.Shared.Abstractions;
using Domain.Features.Orders.Entities;
using Domain.Features.Orders.Repository;
using Domain.Orders.Enums;
using FluentResults;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Application.Features.Orders.Commands.RefundPixTransfer;

/// <summary>
/// Handler for initiating PIX refund transfers
/// </summary>
public class RefundPixTransferCommandHandler : ICommandHandler<RefundPixTransferCommand, RefundPixTransferResponse>
{
    private readonly IRefundTransactionRepository _transactionRepository;
    private readonly IPixTransferService _pixTransferService;
    private readonly ILogger<RefundPixTransferCommandHandler> _logger;
    private readonly IValidator<RefundPixTransferCommand> _validator;
    private readonly string _className = nameof(RefundPixTransferCommandHandler);

    private const int MaxRetryAttempts = 3;
    private static readonly TimeSpan[] RetryDelays = { TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(120) };

    public RefundPixTransferCommandHandler(
        ILogger<RefundPixTransferCommandHandler> logger,
        IValidator<RefundPixTransferCommand> validator,
        IRefundTransactionRepository transactionRepository,
        IPixTransferService pixTransferService)
    {
        _logger = logger;
        _validator = validator;
        _transactionRepository = transactionRepository;
        _pixTransferService = pixTransferService;
    }

    public async Task<Result<RefundPixTransferResponse>> Handle(RefundPixTransferCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "[{ClassName}] Processing PIX refund for OrderId: {OrderId}, Amount: {Amount}",
            _className, request.OrderId, request.RefundAmount);

        try
        {
            // Validate command
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                return Result.Fail(validationResult.Errors.Select(x => x.ErrorMessage));

            // Skip if refund amount is zero
            if (request.RefundAmount <= 0)
            {
                _logger.LogInformation(
                    "[{ClassName}] Skipping refund for OrderId: {OrderId} - zero or negative amount",
                    _className, request.OrderId);
                return Result.Fail("Refund amount must be greater than zero");
            }

            // Create refund transaction record
            var transaction = RefundTransaction.Create(request.OrderId, request.RefundAmount, request.CustomerWalletId);
            await _transactionRepository.SaveAsync(transaction);

            _logger.LogInformation(
                "[{ClassName}] Created refund transaction {TransactionId} for OrderId: {OrderId}",
                _className, transaction.TransactionId, request.OrderId);

            // Attempt PIX transfer with retry logic
            PixTransferResponse? transferResponse = null;
            Exception? lastException = null;

            for (int attempt = 0; attempt < MaxRetryAttempts; attempt++)
            {
                try
                {
                    transferResponse = await _pixTransferService.TransferRefundAsync(
                        request.OrderId,
                        request.RefundAmount,
                        request.CustomerWalletId,
                        cancellationToken);

                    // Success - update transaction and emit event
                    transaction.SetCompleted(transferResponse.id);
                    await _transactionRepository.UpdateAsync(transaction);

                    _logger.LogInformation(
                        "[{ClassName}] PIX transfer successful for OrderId: {OrderId}, TransferId: {TransferId}",
                        _className, request.OrderId, transferResponse.id);

                    return Result.Ok(new RefundPixTransferResponse(
                        transaction.TransactionId,
                        request.OrderId,
                        request.RefundAmount,
                        request.CustomerWalletId,
                        "Completed",
                        transferResponse.id));
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    transaction.IncrementRetryCount();
                    await _transactionRepository.UpdateAsync(transaction);

                    _logger.LogWarning(
                        "[{ClassName}] PIX transfer attempt {Attempt} failed for OrderId: {OrderId}. Error: {Error}",
                        _className, attempt + 1, request.OrderId, ex.Message);

                    // If not the last attempt, wait before retrying
                    if (attempt < MaxRetryAttempts - 1)
                    {
                        await Task.Delay(RetryDelays[attempt], cancellationToken);
                    }
                }
            }

            // All retries exhausted - mark as failed
            transaction.SetFailed(lastException?.Message ?? "Unknown error", transaction.RetryCount);
            await _transactionRepository.UpdateAsync(transaction);

            _logger.LogError(
                "[{ClassName}] PIX transfer failed after {MaxAttempts} attempts for OrderId: {OrderId}. Error: {Error}",
                _className, MaxRetryAttempts, request.OrderId, lastException?.Message);

            return Result.Fail($"PIX transfer failed after {MaxRetryAttempts} attempts: {lastException?.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                "[{ClassName}] Unexpected error processing PIX refund for OrderId: {OrderId}. Error: {Error}",
                _className, request.OrderId, ex);
            return Result.Fail(ex.Message);
        }
    }
}