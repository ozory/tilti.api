using Domain.Features.Orders.Entities;
using Domain.Features.Orders.ValueObjects;
using Domain.Shared.ValueObjects;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Application.Features.Orders.Services;

namespace Tests.Application.Features.Orders.Services;

public class RefundPenaltyCalculationServiceTests
{
    private readonly RefundPenaltyCalculationService _service;
    private readonly Mock<ILogger<RefundPenaltyCalculationService>> _mockLogger;

    public RefundPenaltyCalculationServiceTests()
    {
        _mockLogger = new Mock<ILogger<RefundPenaltyCalculationService>>();
        _service = new RefundPenaltyCalculationService(_mockLogger.Object);
    }

    private Order CreateTestOrder(long orderId = 1, decimal amount = 20.00m)
    {
        var order = Order.Create(orderId);
        order.SetAmount(amount);
        return order;
    }

    #region Core Penalty Calculation Tests

    [Fact]
    public void CalculatePenalty_At0PercentDistance_ReturnsNoPenalty()
    {
        // Arrange
        var order = CreateTestOrder(1, 20.00m);
        var distanceRatio = 0.00m;

        // Act
        var result = _service.CalculatePenalty(order, distanceRatio);

        // Assert
        Assert.Equal(20.00m, result.OriginalAmount);
        Assert.Equal(0, result.PenaltyPercentage);
        Assert.Equal(0, result.PenaltyAmount);
        Assert.Equal(20.00m, result.RefundAmount);
    }

    [Fact]
    public void CalculatePenalty_At50PercentDistance_ReturnsApprox30To40PercentPenalty()
    {
        // Arrange
        var order = CreateTestOrder(1, 20.00m);
        var distanceRatio = 0.50m;

        // Act
        var result = _service.CalculatePenalty(order, distanceRatio);

        // Assert
        Assert.Equal(20.00m, result.OriginalAmount);
        Assert.InRange(result.PenaltyPercentage, 30, 40);
        Assert.InRange(result.RefundAmount, 12.00m, 14.00m); // 60-70% of 20
    }

    [Fact]
    public void CalculatePenalty_At90PercentDistance_ReturnsApprox70To80PercentPenalty()
    {
        // Arrange
        var order = CreateTestOrder(1, 20.00m);
        var distanceRatio = 0.90m;

        // Act
        var result = _service.CalculatePenalty(order, distanceRatio);

        // Assert
        Assert.Equal(20.00m, result.OriginalAmount);
        Assert.InRange(result.PenaltyPercentage, 70, 80);
        Assert.InRange(result.RefundAmount, 4.00m, 6.00m); // 20-30% of 20
    }

    [Fact]
    public void CalculatePenalty_At100PercentDistance_ReturnsCapped80PercentPenalty()
    {
        // Arrange
        var order = CreateTestOrder(1, 20.00m);
        var distanceRatio = 1.00m;

        // Act
        var result = _service.CalculatePenalty(order, distanceRatio);

        // Assert
        Assert.Equal(20.00m, result.OriginalAmount);
        Assert.Equal(80, result.PenaltyPercentage);
        Assert.Equal(4.00m, result.RefundAmount); // Minimum 20%
    }

    #endregion

    #region Minimum Refund Rule Tests

    [Fact]
    public void CalculatePenalty_EnforcesMinimum20PercentRefund()
    {
        // Arrange
        var order = CreateTestOrder(1, 50.00m);
        var distanceRatio = 1.00m; // At 100% distance

        // Act
        var result = _service.CalculatePenalty(order, distanceRatio);

        // Assert
        Assert.Equal(50.00m, result.OriginalAmount);
        // At 100% distance, penalty should be capped at 80%, but allow some tolerance for rounding
        Assert.InRange(result.PenaltyPercentage, 77, 80);
        // Minimum 20% refund
        Assert.True(result.RefundAmount >= result.OriginalAmount * 0.20m);
    }

    #endregion

    #region Distance Ratio Edge Cases

    [Fact]
    public void CalculatePenalty_WithNegativeDistanceRatio_ClampsToZero()
    {
        // Arrange
        var order = CreateTestOrder(1, 20.00m);
        var distanceRatio = -0.5m;

        // Act
        var result = _service.CalculatePenalty(order, distanceRatio);

        // Assert
        Assert.Equal(0, result.PenaltyPercentage);
        Assert.Equal(20.00m, result.RefundAmount);
    }

    [Fact]
    public void CalculatePenalty_WithDistanceRatioGreaterThan1_ClampsTo1()
    {
        // Arrange
        var order = CreateTestOrder(1, 20.00m);
        var distanceRatio = 1.5m;

        // Act
        var result = _service.CalculatePenalty(order, distanceRatio);

        // Assert
        Assert.Equal(80, result.PenaltyPercentage);
        Assert.Equal(4.00m, result.RefundAmount);
    }

    #endregion

    #region Various Amount Tests

    [Theory]
    [InlineData(10.00)]
    [InlineData(50.00)]
    [InlineData(100.00)]
    [InlineData(150.50)]
    public void CalculatePenalty_WithVariousAmounts_CalculatesCorrectly(decimal amount)
    {
        // Arrange
        var order = CreateTestOrder(1, (decimal)amount);
        var distanceRatio = 0.50m;

        // Act
        var result = _service.CalculatePenalty(order, distanceRatio);

        // Assert
        Assert.Equal((decimal)amount, result.OriginalAmount);
        Assert.InRange(result.PenaltyPercentage, 30, 40);
        var expectedRefund = (decimal)amount * 0.60m; // ~60% of amount
        Assert.InRange(result.RefundAmount, expectedRefund - (decimal)amount * 0.10m, expectedRefund + (decimal)amount * 0.10m);
    }

    #endregion

    #region Distance Reference Point Tests

    [Theory]
    [InlineData(0.00, 0)]      // 0% distance: 0% penalty
    [InlineData(0.25, 12.5)]   // 25% distance: ~12.5% penalty
    [InlineData(0.50, 35)]     // 50% distance: ~35% penalty
    [InlineData(0.75, 65)]     // 75% distance: ~65% penalty
    public void CalculatePenalty_AtReferencePoints_ReturnsExpectedPenalty(double distanceRatio, decimal expectedPenalty)
    {
        // Arrange
        var order = CreateTestOrder(1, 100.00m);
        var distanceRatioDecimal = (decimal)distanceRatio;

        // Act
        var result = _service.CalculatePenalty(order, distanceRatioDecimal);

        // Assert
        // Allow some tolerance for rounding
        Assert.InRange(result.PenaltyPercentage, expectedPenalty - 5, expectedPenalty + 5);
    }

    #endregion
}
