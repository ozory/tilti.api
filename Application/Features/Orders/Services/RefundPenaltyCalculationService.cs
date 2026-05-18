using Domain.Features.Orders.ValueObjects;
using Domain.Features.Orders.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Features.Orders.Services;

/// <summary>
/// Service responsible for calculating refund penalties based on driver distance ratio.
/// </summary>
public class RefundPenaltyCalculationService
{
    private readonly ILogger<RefundPenaltyCalculationService> _logger;
    private readonly string _className = nameof(RefundPenaltyCalculationService);

    public RefundPenaltyCalculationService(ILogger<RefundPenaltyCalculationService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Calculates the refund penalty for a canceled order based on driver distance ratio.
    /// 
    /// Penalty Rules (based on distance traveled):
    /// - 0% distance: 0% penalty
    /// - 25% distance: 10-15% penalty
    /// - 50% distance: 30-40% penalty
    /// - 75% distance: 60-70% penalty
    /// - 90%+ distance: 70-80% penalty
    /// - Maximum penalty: 80%
    /// - Minimum refund: 20%
    /// - No penalty if driver never accepted the order
    /// </summary>
    /// <param name="order">The canceled order</param>
    /// <param name="distanceRatio">Distance ratio (0.0 to 1.0)</param>
    /// <returns>Penalty calculation result</returns>
    public PenaltyCalculationResult CalculatePenalty(Order order, decimal distanceRatio)
    {
        _logger.LogInformation("[{className}] Calculating penalty for OrderId: {OrderId}, DistanceRatio: {DistanceRatio}",
            _className, order.Id, distanceRatio);

        // Clamp distance ratio to valid range
        var clampedRatio = Math.Max(0, Math.Min(1, (double)distanceRatio));

        // Calculate penalty percentage based on distance ratio
        decimal penaltyPercentage = CalculatePenaltyPercentage(clampedRatio);

        // Create and return the penalty calculation result
        var result = PenaltyCalculationResult.Create(order.Amount.Value, penaltyPercentage, distanceRatio);

        _logger.LogInformation(
            "[{className}] Penalty calculated - OrderId: {OrderId}, OriginalAmount: {Amount}, PenaltyPercentage: {Penalty}%, RefundAmount: {Refund}",
            _className, order.Id, result.OriginalAmount, result.PenaltyPercentage, result.RefundAmount);

        return result;
    }

    /// <summary>
    /// Calculates penalty percentage based on distance ratio.
    /// Uses linear interpolation with the following reference points:
    /// - 0% distance: 0% penalty
    /// - 25% distance: ~12.5% penalty (average of 10-15%)
    /// - 50% distance: ~35% penalty (average of 30-40%)
    /// - 75% distance: ~65% penalty (average of 60-70%)
    /// - 100% distance: ~75% penalty (average of 70-80%, capped at 80%)
    /// </summary>
    private decimal CalculatePenaltyPercentage(double distanceRatio)
    {
        decimal penalty = distanceRatio switch
        {
            // 0-25% distance: linear from 0% to 12.5% penalty
            >= 0 and < 0.25 => (decimal)(distanceRatio * 50),

            // 25-50% distance: linear from 12.5% to 35% penalty
            >= 0.25 and < 0.50 => (decimal)(12.5 + (distanceRatio - 0.25) * 90),

            // 50-75% distance: linear from 35% to 65% penalty
            >= 0.50 and < 0.75 => (decimal)(35 + (distanceRatio - 0.50) * 120),

            // 75-100% distance: linear from 65% to 80% penalty (capped)
            >= 0.75 => (decimal)(65 + (distanceRatio - 0.75) * 60),

            _ => 0
        };

        // Ensure penalty doesn't exceed 80%
        return Math.Min(80, penalty);
    }
}
