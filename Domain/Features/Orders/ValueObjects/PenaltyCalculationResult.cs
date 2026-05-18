using Domain.Abstractions;

namespace Domain.Features.Orders.ValueObjects;

/// <summary>
/// Value object representing the result of a refund penalty calculation.
/// </summary>
public class PenaltyCalculationResult
{
    public decimal OriginalAmount { get; private set; }
    public decimal PenaltyPercentage { get; private set; }
    public decimal PenaltyAmount { get; private set; }
    public decimal RefundAmount { get; private set; }
    public decimal DistanceRatio { get; private set; }

    private PenaltyCalculationResult() { }

    public static PenaltyCalculationResult Create(
        decimal originalAmount,
        decimal penaltyPercentage,
        decimal distanceRatio)
    {
        // Validate that penalty is between 0 and 80
        if (penaltyPercentage < 0 || penaltyPercentage > 80)
            throw new ArgumentException("Penalty percentage must be between 0 and 80");

        // Calculate penalty and refund amounts
        var penaltyAmount = originalAmount * (penaltyPercentage / 100);
        var refundAmount = originalAmount - penaltyAmount;

        // Enforce minimum 20% refund (maximum 80% penalty)
        if (refundAmount < originalAmount * 0.20m)
        {
            refundAmount = originalAmount * 0.20m;
            penaltyAmount = originalAmount - refundAmount;
            penaltyPercentage = (penaltyAmount / originalAmount) * 100;
        }

        return new PenaltyCalculationResult
        {
            OriginalAmount = originalAmount,
            PenaltyPercentage = penaltyPercentage,
            PenaltyAmount = penaltyAmount,
            RefundAmount = refundAmount,
            DistanceRatio = distanceRatio
        };
    }
}

