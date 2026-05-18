using Application.Features.Orders.Commands.CancelOrder;
using Application.Features.Orders.Contracts;
using Application.Shared.Abstractions;
using Domain.Enums;
using Domain.Features.Orders.Entities;
using Domain.Features.Orders.Repository;
using Domain.Features.Users.Repository;
using Domain.Orders.Enums;
using Domain.Shared.Abstractions;
using Domain.Shared.ValueObjects;
using Domain.ValueObjects;
using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using NetTopologySuite.Geometries;
using Domain.Features.Orders.Events;

namespace Tests.Application.Features.Orders.Commands;

public class CancelOrderCommandHandlerTests
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IValidator<CancelOrderCommand>> _validatorMock;
    private readonly Mock<IMessageRepository> _messageRepositoryMock;
    private readonly Mock<ILogger<CancelOrderCommandHandler>> _loggerMock;
    private readonly CancelOrderCommandHandler _handler;

    public CancelOrderCommandHandlerTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _validatorMock = new Mock<IValidator<CancelOrderCommand>>();
        _messageRepositoryMock = new Mock<IMessageRepository>();
        _loggerMock = new Mock<ILogger<CancelOrderCommandHandler>>();

        // The handler no longer requires IMessageRepository or IConfiguration directly.
        _handler = new CancelOrderCommandHandler(
            _loggerMock.Object,
            _orderRepositoryMock.Object,
            _validatorMock.Object,
            _userRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidPendingOrder_ShouldCancelSuccessfully()
    {
        // Arrange
        var orderId = 123L;
        var userId = 456L;
        var order = CreateTestOrder(orderId, userId, OrderStatus.ReadyToAccept);

        var command = new CancelOrderCommand(
            UserId: userId,
            OrderId: orderId,
            requestedTime: DateTime.UtcNow,
            reason: new List<string> { "changed_mind" },
            description: "Customer changed their mind",
            cancelledBy: "User"
        );

        _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _userRepositoryMock.Setup(u => u.GetByIdAsync(userId))
            .ReturnsAsync(CreateTestUser(userId));

        _orderRepositoryMock.Setup(r => r.GetByIdAsync(orderId))
            .ReturnsAsync(order);

        _orderRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Order>()))
            .ReturnsAsync((Order o) => o);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(orderId);
        result.Value.Status.Should().Be("Canceled");
        result.Value.CancelledBy.Should().Be("User");

        _orderRepositoryMock.Verify(r => r.UpdateAsync(It.Is<Order>(o =>
            o.DomainEvents.Any(e => e is OrderCanceledDomainEvent))), Times.Once);
    }

    [Fact]
    public async Task Handle_WithInTransitOrder_ShouldReturnFailure()
    {
        // Arrange
        var orderId = 123L;
        var userId = 456L;
        var order = CreateTestOrder(orderId, userId, OrderStatus.InTransit);

        var command = new CancelOrderCommand(
            UserId: userId,
            OrderId: orderId,
            requestedTime: DateTime.UtcNow,
            cancelledBy: "User"
        );

        _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _userRepositoryMock.Setup(u => u.GetByIdAsync(userId))
            .ReturnsAsync(CreateTestUser(userId));

        _orderRepositoryMock.Setup(r => r.GetByIdAsync(orderId))
            .ReturnsAsync(order);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Message.Contains("Não é possível cancelar um pedido em andamento"));
    }

    [Fact]
    public async Task Handle_WithAlreadyCanceledOrder_ShouldReturnFailure()
    {
        // Arrange
        var orderId = 123L;
        var userId = 456L;
        var order = CreateTestOrder(orderId, userId, OrderStatus.Canceled);

        var command = new CancelOrderCommand(
            UserId: userId,
            OrderId: orderId,
            requestedTime: DateTime.UtcNow,
            cancelledBy: "User"
        );

        _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _userRepositoryMock.Setup(u => u.GetByIdAsync(userId))
            .ReturnsAsync(CreateTestUser(userId));

        _orderRepositoryMock.Setup(r => r.GetByIdAsync(orderId))
            .ReturnsAsync(order);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Message.Contains("Pedido já foi cancelado"));
    }

    [Fact]
    public async Task Handle_WithFinishedOrder_ShouldReturnFailure()
    {
        // Arrange
        var orderId = 123L;
        var userId = 456L;
        var order = CreateTestOrder(orderId, userId, OrderStatus.Finished);

        var command = new CancelOrderCommand(
            UserId: userId,
            OrderId: orderId,
            requestedTime: DateTime.UtcNow,
            cancelledBy: "User"
        );

        _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _userRepositoryMock.Setup(u => u.GetByIdAsync(userId))
            .ReturnsAsync(CreateTestUser(userId));

        _orderRepositoryMock.Setup(r => r.GetByIdAsync(orderId))
            .ReturnsAsync(order);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Message.Contains("Pedido já foi Finalizado"));
    }

    [Fact]
    public async Task Handle_WithNonExistentOrder_ShouldReturnFailure()
    {
        // Arrange
        var orderId = 999L;
        var userId = 456L;

        var command = new CancelOrderCommand(
            UserId: userId,
            OrderId: orderId,
            requestedTime: DateTime.UtcNow,
            cancelledBy: "User"
        );

        _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _userRepositoryMock.Setup(u => u.GetByIdAsync(userId))
            .ReturnsAsync(CreateTestUser(userId));

        _orderRepositoryMock.Setup(r => r.GetByIdAsync(orderId))
            .ReturnsAsync((Order?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Message.Contains("Nenhum pedido encontrado"));
    }

    [Fact]
    public async Task Handle_WithUnauthorizedUser_ShouldReturnFailure()
    {
        // Arrange
        var orderId = 123L;
        var orderUserId = 456L;
        var requestingUserId = 789L; // Different user
        var order = CreateTestOrder(orderId, orderUserId, OrderStatus.ReadyToAccept);

        var command = new CancelOrderCommand(
            UserId: requestingUserId,
            OrderId: orderId,
            requestedTime: DateTime.UtcNow,
            cancelledBy: "User"
        );

        _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _userRepositoryMock.Setup(u => u.GetByIdAsync(requestingUserId))
            .ReturnsAsync(CreateTestUser(requestingUserId));

        _orderRepositoryMock.Setup(r => r.GetByIdAsync(orderId))
            .ReturnsAsync(order);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Message.Contains("Usuário não autorizado a cancelar este pedido"));
    }

    [Fact]
    public async Task Handle_WithDriverCancellation_ShouldNotPublishRefundEvent()
    {
        // Arrange
        var orderId = 123L;
        var driverId = 456L;
        var order = CreateTestOrder(orderId, 789L, OrderStatus.ReadyToAccept);
        order.SetDriver(CreateTestUser(driverId));

        var command = new CancelOrderCommand(
            UserId: driverId,
            OrderId: orderId,
            requestedTime: DateTime.UtcNow,
            reason: new List<string> { "emergency" },
            description: "Driver emergency",
            cancelledBy: "Driver"
        );

        _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _userRepositoryMock.Setup(u => u.GetByIdAsync(driverId))
            .ReturnsAsync(CreateTestUser(driverId));

        _orderRepositoryMock.Setup(r => r.GetByIdAsync(orderId))
            .ReturnsAsync(order);

        _orderRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Order>()))
            .ReturnsAsync((Order o) => o);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be("Canceled");
        result.Value.CancelledBy.Should().Be("Driver");

        // Should NOT publish refund event for driver cancellation
        _messageRepositoryMock.Verify(m => m.PublishAsync(
            It.IsAny<IDomainEvent>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithDefaultCancelledBy_ShouldUseUser()
    {
        // Arrange
        var orderId = 123L;
        var userId = 456L;
        var order = CreateTestOrder(orderId, userId, OrderStatus.ReadyToAccept);

        var command = new CancelOrderCommand(
            UserId: userId,
            OrderId: orderId,
            requestedTime: DateTime.UtcNow,
            cancelledBy: null // null should default to User
        );

        _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _userRepositoryMock.Setup(u => u.GetByIdAsync(userId))
            .ReturnsAsync(CreateTestUser(userId));

        _orderRepositoryMock.Setup(r => r.GetByIdAsync(orderId))
            .ReturnsAsync(order);

        _orderRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Order>()))
            .ReturnsAsync((Order o) => o);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.CancelledBy.Should().Be("User");
    }

    private Order CreateTestOrder(long orderId, long userId, OrderStatus status)
    {
        var order = Order.Create(orderId);
        order.SetUser(CreateTestUser(userId));
        order.SetAmount(25.50m);
        order.SetStatus(status);
        order.SetDistanceInKM(10);
        order.SetDurationInSeconds(600);
        order.SetLocation(-23.5505, -46.6333);
        return order;
    }

    private Domain.Features.Users.Entities.User CreateTestUser(long userId)
    {
        var user = Domain.Features.Users.Entities.User.Create(
            id: userId,
            name: "Test User",
            email: "test@example.com",
            document: "12345678901",
            password: "password123",
            createdDate: DateTime.UtcNow
        );
        user.SetStatus(Domain.Enums.UserStatus.Active);
        return user;
    }
}