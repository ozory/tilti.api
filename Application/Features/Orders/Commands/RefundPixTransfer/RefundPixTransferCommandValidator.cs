using FluentValidation;

namespace Application.Features.Orders.Commands.RefundPixTransfer;

/// <summary>
/// Validator for RefundPixTransferCommand
/// </summary>
public class RefundPixTransferCommandValidator : AbstractValidator<RefundPixTransferCommand>
{
    public RefundPixTransferCommandValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.OrderId)
            .GreaterThan(0)
            .WithMessage("OrderId must be greater than 0");

        RuleFor(x => x.RefundAmount)
            .GreaterThan(0)
            .WithMessage("RefundAmount must be greater than 0");

        RuleFor(x => x.CustomerWalletId)
            .NotEmpty()
            .WithMessage("CustomerWalletId is required")
            .Must(BeValidUuid)
            .WithMessage("CustomerWalletId must be a valid UUID format");
    }

    private static bool BeValidUuid(string walletId)
    {
        if (string.IsNullOrWhiteSpace(walletId))
            return false;

        return Guid.TryParse(walletId, out _);
    }
}