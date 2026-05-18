using Application.Features.Orders.Commands.RefundPixTransfer;
using Application.Shared.Abstractions;
using Domain.Features.Orders.Entities;
using Domain.Features.Orders.Repository;
using Domain.Orders.Enums;
using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Application.Features.Orders.Commands;

public class RefundPixTransferCommandHandlerTests
{
    private readonly Mock<IRefundTransactionRepository> _transactionRepositoryMock;
    private readonly Mock<IPixTransferService> _pixTransferServiceMock;
    private readonly Mock<IValidator<RefundPixTransferCommand>> _validatorMock;
    private readonly Mock<ILogger<RefundPixTransferCommandHandler>> _loggerMock;
    private readonly RefundPixTransferCommandHandler _handler;

    public RefundPixTransferCommandHandlerTests()
    {
        _transactionRepositoryMock = new Mock<IRefundTransactionRepository>();
        _pixTransferServiceMock = new Mock<IPixTransferService>();
        _validatorMock = new Mock<IValidator<RefundPixTransferCommand>>();
        _loggerMock = new Mock<ILogger<RefundPixTransferCommandHandler>>();

        _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<RefundPixTransferCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _handler = new RefundPixTransferCommandHandler(
            _loggerMock.Object,
            _validatorMock.Object,
            _transactionRepositoryMock.Object,
            _pixTransferServiceMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldReturnSuccess()
    {
        // Arrange
        var orderId = 123L;
        var amount = 50.00m;
        var walletId = "12345678-1234-1234-1234-123456789012";
        var command = new RefundPixTransferCommand(orderId, amount, walletId);

        var transferResponse = new PixTransferResponse(
            id: "transfer-123",
            status: "CONFIRMED",
            value: amount,
            pixTransferId: null,
            walletId: walletId,
            dateCreated: DateTime.UtcNow,
            externalReference: $"refund-{orderId}");

        _pixTransferServiceMock
            .Setup(s => s.TransferRefundAsync(orderId, amount, walletId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(transferResponse);

        _transactionRepositoryMock
            .Setup(r => r.SaveAsync(It.IsAny<RefundTransaction>()))
            .ReturnsAsync((RefundTransaction t) => t);

        _transactionRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<RefundTransaction>()))
            .ReturnsAsync((RefundTransaction t) => t);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.OrderId.Should().Be(orderId);
        result.Value.Amount.Should().Be(amount);
        result.Value.CustomerWalletId.Should().Be(walletId);
        result.Value.Status.Should().Be("Completed");

        _pixTransferServiceMock.Verify(s => s.TransferRefundAsync(orderId, amount, walletId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithZeroAmount_ShouldReturnFailure()
    {
        // Arrange
        var orderId = 123L;
        var amount = 0m;
        var walletId = "12345678-1234-1234-1234-123456789012";
        var command = new RefundPixTransferCommand(orderId, amount, walletId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Message.Contains("greater than zero"));

        _pixTransferServiceMock.Verify(s => s.TransferRefundAsync(It.IsAny<long>(), It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithNegativeAmount_ShouldReturnFailure()
    {
        // Arrange
        var orderId = 123L;
        var amount = -10m;
        var walletId = "12345678-1234-1234-1234-123456789012";
        var command = new RefundPixTransferCommand(orderId, amount, walletId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Message.Contains("greater than zero"));

        _pixTransferServiceMock.Verify(s => s.TransferRefundAsync(It.IsAny<long>(), It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithInvalidWalletId_ShouldReturnFailure()
    {
        // Arrange
        var orderId = 123L;
        var amount = 50.00m;
        var walletId = "invalid-uuid";
        var command = new RefundPixTransferCommand(orderId, amount, walletId);

        var validationResult = new FluentValidation.Results.ValidationResult();
        validationResult.Errors.Add(new FluentValidation.Results.ValidationFailure("CustomerWalletId", "CustomerWalletId must be a valid UUID format"));

        _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<RefundPixTransferCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();

        _pixTransferServiceMock.Verify(s => s.TransferRefundAsync(It.IsAny<long>(), It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithPixTransferFailure_ShouldRetryAndReturnFailure()
    {
        // Arrange
        var orderId = 123L;
        var amount = 50.00m;
        var walletId = "12345678-1234-1234-1234-123456789012";
        var command = new RefundPixTransferCommand(orderId, amount, walletId);

        _pixTransferServiceMock
            .Setup(s => s.TransferRefundAsync(orderId, amount, walletId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("PIX transfer failed"));

        _transactionRepositoryMock
            .Setup(r => r.SaveAsync(It.IsAny<RefundTransaction>()))
            .ReturnsAsync((RefundTransaction t) => t);

        _transactionRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<RefundTransaction>()))
            .ReturnsAsync((RefundTransaction t) => t);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Message.Contains("PIX transfer failed after 3 attempts"));

        // Verify 3 attempts were made
        _pixTransferServiceMock.Verify(s => s.TransferRefundAsync(orderId, amount, walletId, It.IsAny<CancellationToken>()), Times.Exactly(3));
    }

    [Fact]
    public async Task Handle_WithTransientFailure_ThenSuccess_ShouldReturnSuccess()
    {
        // Arrange
        var orderId = 123L;
        var amount = 50.00m;
        var walletId = "12345678-1234-1234-1234-123456789012";
        var command = new RefundPixTransferCommand(orderId, amount, walletId);

        var transferResponse = new PixTransferResponse(
            id: "transfer-123",
            status: "CONFIRMED",
            value: amount,
            pixTransferId: null,
            walletId: walletId,
            dateCreated: DateTime.UtcNow,
            externalReference: $"refund-{orderId}");

        var callCount = 0;
        _pixTransferServiceMock
            .Setup(s => s.TransferRefundAsync(orderId, amount, walletId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                callCount++;
                if (callCount == 1)
                    throw new Exception("Transient error");
                return transferResponse;
            });

        _transactionRepositoryMock
            .Setup(r => r.SaveAsync(It.IsAny<RefundTransaction>()))
            .ReturnsAsync((RefundTransaction t) => t);

        _transactionRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<RefundTransaction>()))
            .ReturnsAsync((RefundTransaction t) => t);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();

        // Verify 2 attempts were made (1 failure + 1 success)
        _pixTransferServiceMock.Verify(s => s.TransferRefundAsync(orderId, amount, walletId, It.IsAny<CancellationToken>()), Times.Exactly(2));
    }
}