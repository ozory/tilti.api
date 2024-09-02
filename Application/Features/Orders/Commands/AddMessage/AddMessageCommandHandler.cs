using Application.Features.Orders.Commands.AddMessage;
using Application.Features.Orders.Contracts;
using Application.Features.Users.Commands.CreateUser;
using Application.Shared.Abstractions;
using Domain.Enums;
using Domain.Features.Orders.Entities;
using Domain.Features.Orders.Events;
using Domain.Features.Orders.Repository;
using Domain.Features.Users.Repository;
using Domain.Shared.Abstractions;
using FluentResults;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Application.Features.Orders.Commands.AddMessage;

public class AddMessageCommandHandler : ICommandHandler<AddMessageCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AddMessageCommandHandler> _logger;
    private readonly IValidator<AddMessageCommand> _validator;
    private readonly string className = nameof(AddMessageCommandHandler);

    public AddMessageCommandHandler(
        ILogger<AddMessageCommandHandler> logger,
        IValidator<AddMessageCommand> validator,
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _validator = validator;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(AddMessageCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("[{className}] Add mensage to Order {OrderId}", className, request.OrderId);

        try
        {
            var validationResult = _validator.Validate(request);
            if (!validationResult.IsValid) return Result.Fail(validationResult.Errors.Select(x => x.ErrorMessage));

            var userValidate = await CreateUserCommandValidator.ValidateUser(_unitOfWork.UserRepository, request.SourceUserId);
            if (userValidate.IsFailed) return Result.Fail(userValidate.Errors);

            var driverValidate = await CreateUserCommandValidator.ValidateUser(_unitOfWork.UserRepository, request.TargetUserId);
            if (userValidate.IsFailed) return Result.Fail(userValidate.Errors);

            var order = await _unitOfWork.OrderRepository.GetByIdAsync(request.OrderId);
            if (order == null ||
                (
                    order.Status == OrderStatus.Finished
                    || order.Status == OrderStatus.Canceled
                    || order.Status == OrderStatus.Expired)) { return Result.Fail("Ordem já finalizada ou expirada"); }

            var user = userValidate.Value;
            var message = Message.Create(request.SourceUserId, request.TargetUserId, request.OrderId, request.Message);

            // Save user
            var savedMessage = await _unitOfWork.OrderMessageRepository.SaveAsync(message);
            savedMessage.AddDomainEvent((CreateMessageDomainEvent)savedMessage);
            await _unitOfWork.CommitAsync(cancellationToken);

            _logger.LogInformation("[{className}] Order Message Created {savedMessage}", className, savedMessage.Id);
            return Result.Ok(true);
        }
        catch (Exception ex)
        {
            _logger.LogError("[{className}] Error creating Order :{request} Error: {ex}", className, request, ex);
            throw;
        }

    }
}
