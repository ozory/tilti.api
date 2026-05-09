using Application.Features.Orders.Contracts;
using Application.Features.Payments.Contracts;
using Application.Shared.Abstractions;
using Domain.Features.Payments.Entities;
using Domain.Features.Users.Repository;
using Domain.Shared.Abstractions;
using Domain.ValueObjects;
using FluentResults;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Application.Features.Payments.Commands.CreatePassengerPayment;

/// <summary>
/// Handler for creating passenger payments
/// </summary>
public class CreatePassengerPaymentCommandHandler : ICommandHandler<CreatePassengerPaymentCommand, PaymentResponse>
{
    private readonly IPassengerPaymentService _paymentService;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreatePassengerPaymentCommandHandler> _logger;
    private readonly IValidator<CreatePassengerPaymentCommand> _validator;
    private readonly string _className = nameof(CreatePassengerPaymentCommandHandler);

    public CreatePassengerPaymentCommandHandler(
        ILogger<CreatePassengerPaymentCommandHandler> logger,
        IPassengerPaymentService paymentService,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IValidator<CreatePassengerPaymentCommand> validator)
    {
        _logger = logger;
        _paymentService = paymentService;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    /// <summary>
    /// Handles the creation of a passenger payment
    /// </summary>
    public async Task<Result<PaymentResponse>> Handle(
        CreatePassengerPaymentCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("[{ClassName}] Creating passenger payment for User {UserId}, Amount: {Amount}",
            _className, request.UserId, request.Amount);

        try
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                return Result.Fail(validationResult.Errors.Select(x => x.ErrorMessage));

            // Get user
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
                return Result.Fail("User not found");

            // Create or get wallet
            var walletId = await _paymentService.CreateOrGetWalletAsync(request.UserId, cancellationToken);

            // Create payment in Asaas
            var (paymentId, pixLink, qrCode) = await _paymentService.CreatePaymentAsync(
                orderId: 0, // Will be set after order creation
                amount: request.Amount,
                customerWalletId: walletId,
                dueDate: request.DueDate,
                cancellationToken: cancellationToken
            );

            // Create and save payment record in database
            var payment = Payment.Create(
                id: null,
                identifier: null,
                user: user,
                type: Domain.Features.Payments.Enums.PaymentType.Pix,
                requestedTime: DateTime.Now);

            payment.SetAsaasPaymentId(paymentId);
            payment.SetPixLink(pixLink);
            payment.SetPixQrCode(qrCode);
            payment.SetAmount(new Amount(request.Amount));

            await _unitOfWork.PaymentRepository.SaveAsync(payment);
            await _unitOfWork.CommitAsync(cancellationToken);

            _logger.LogInformation("[{ClassName}] Payment created: {PaymentId} for User {UserId}",
                _className, paymentId, request.UserId);

            return Result.Ok(new PaymentResponse
            {
                PaymentId = paymentId,
                PixLink = pixLink,
                QrCode = qrCode,
                Amount = request.Amount,
                Status = "PENDING",
                DueDate = request.DueDate
            });
        }
        catch (Exception ex)
        {
            _logger.LogError("[{ClassName}] Error creating passenger payment: {Exception}",
                _className, ex);
            return Result.Fail($"Error creating payment: {ex.Message}");
        }
    }
}
