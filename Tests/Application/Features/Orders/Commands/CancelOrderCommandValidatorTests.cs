using Application.Features.Orders.Commands.CancelOrder;
using Domain.Features.Users.Repository;
using Domain.Orders.Enums;
using FluentAssertions;
using Moq;

namespace Tests.Application.Features.Orders.Commands;

public class CancelOrderCommandValidatorTests
{
    [Fact]
    public void Validate_WithOptionalDescriptionAndReason_ShouldBeValid()
    {
        var userRepository = new Mock<IUserRepository>();
        var validator = new CancelOrderCommandValidator(userRepository.Object);

        var command = new CancelOrderCommand(
            UserId: 123,
            OrderId: 456,
            requestedTime: DateTime.UtcNow,
            reason: null,
            description: null,
            cancelledBy: "User"
        );

        var result = validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithInvalidCancelledBy_ShouldFail()
    {
        var userRepository = new Mock<IUserRepository>();
        var validator = new CancelOrderCommandValidator(userRepository.Object);

        var command = new CancelOrderCommand(
            UserId: 123,
            OrderId: 456,
            requestedTime: DateTime.UtcNow,
            reason: new List<string> { "because" },
            description: "Description",
            cancelledBy: "Invalid"
        );

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(x => x.ErrorMessage.Contains("CancelledBy must be null, 'User', or 'Driver'"));
    }

    [Fact]
    public void ParsedCancelledBy_WithUserString_ShouldReturnUserEnum()
    {
        var command = new CancelOrderCommand(
            UserId: 123,
            OrderId: 456,
            requestedTime: DateTime.UtcNow,
            cancelledBy: "User"
        );

        command.ParsedCancelledBy.Should().Be(CancellationInitiator.User);
    }

    [Fact]
    public void ParsedCancelledBy_WithDriverString_ShouldReturnDriverEnum()
    {
        var command = new CancelOrderCommand(
            UserId: 123,
            OrderId: 456,
            requestedTime: DateTime.UtcNow,
            cancelledBy: "Driver"
        );

        command.ParsedCancelledBy.Should().Be(CancellationInitiator.Driver);
    }

    [Fact]
    public void ParsedCancelledBy_WithNull_ShouldReturnNull()
    {
        var command = new CancelOrderCommand(
            UserId: 123,
            OrderId: 456,
            requestedTime: DateTime.UtcNow,
            cancelledBy: null
        );

        command.ParsedCancelledBy.Should().BeNull();
    }

    [Fact]
    public void ParsedCancelledBy_WithCaseInsensitive_ShouldWorkCorrectly()
    {
        var commandUser = new CancelOrderCommand(
            UserId: 123,
            OrderId: 456,
            requestedTime: DateTime.UtcNow,
            cancelledBy: "user"
        );

        var commandDriver = new CancelOrderCommand(
            UserId: 123,
            OrderId: 456,
            requestedTime: DateTime.UtcNow,
            cancelledBy: "DRIVER"
        );

        commandUser.ParsedCancelledBy.Should().Be(CancellationInitiator.User);
        commandDriver.ParsedCancelledBy.Should().Be(CancellationInitiator.Driver);
    }
}
