using Application.Features.Users.Commands.CreateUser;
using Application.Shared.Abstractions;
using Domain.Features.Users.Entities;
using Domain.Features.Users.Repository;
using Domain.Shared.Abstractions;
using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;

namespace Tests.Application.Features.Users.Commands;

public class CreateUserCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly Mock<ILogger<CreateUserCommandHandler>> _logger;
    private readonly Mock<IValidator<CreateUserCommand>> _validator;
    private readonly Mock<ISecurityExtensions> _securityExtensions;
    private readonly Mock<IUserRepository> _mockUserRepository;

    public CreateUserCommandHandlerTests()
    {
        _unitOfWork = new Mock<IUnitOfWork>();
        _logger = new Mock<ILogger<CreateUserCommandHandler>>();
        _validator = new Mock<IValidator<CreateUserCommand>>();
        _securityExtensions = new Mock<ISecurityExtensions>();
        _mockUserRepository = new Mock<IUserRepository>();

        _unitOfWork.Setup(uow => uow.UserRepository)
                  .Returns(_mockUserRepository.Object);

        _mockUserRepository.Setup(repo => repo.SaveAsync(It.IsAny<User>()))
                 .ReturnsAsync(User.Create(1, "Test User", "teste@teste.com", "123456789", "12345678", DateTime.Now));
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCreateUser()
    {
        // Arrange
        var command = new CreateUserCommand(
             "Test User",
            "test@email.com",
            "12345678",
            "123456789"
        );

        var handler = new CreateUserCommandHandler(
            _unitOfWork.Object,
            _logger.Object,
            _validator.Object,
            _securityExtensions.Object
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _unitOfWork.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}