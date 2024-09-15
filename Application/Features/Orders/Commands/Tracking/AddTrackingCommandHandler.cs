using Application.Features.Orders.Commands.AddMessage;
using Application.Features.Orders.Contracts;
using Application.Features.Users.Commands.CreateUser;
using Application.Shared.Abstractions;
using Domain.Enums;
using Domain.Features.Orders.Entities;
using Domain.Features.Orders.Events;
using Domain.Features.Orders.Repository;
using Domain.Features.Users.Repository;
using Domain.Orders.Enums;
using Domain.Shared.Abstractions;
using Domain.Shared.ValueObjects;
using FluentResults;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Application.Features.Orders.Commands.OrderTracking;

public class AddTrackingCommandHandler : ICommandHandler<AddTrackingCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AddTrackingCommandHandler> _logger;
    private readonly IValidator<AddTrackingCommand> _validator;
    private readonly string className = nameof(AddTrackingCommandHandler);

    public AddTrackingCommandHandler(
        ILogger<AddTrackingCommandHandler> logger,
        IValidator<AddTrackingCommand> validator,
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _validator = validator;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(AddTrackingCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("[{className}] Add mensage to Order {OrderId}", className, request.OrderId);

        try
        {
            var validationResult = _validator.Validate(request);
            if (!validationResult.IsValid) return Result.Fail(validationResult.Errors.Select(x => x.ErrorMessage));

            var order = await _unitOfWork.OrderRepository.GetByIdAsync(request.OrderId);
            if (order == null ||
                (
                    order.Status == OrderStatus.Finished
                    || order.Status == OrderStatus.Canceled
                    || order.Status == OrderStatus.Expired)) { return Result.Fail("Ordem já finalizada ou expirada"); }

            var tracking = Tracking.Create(order.Id, new Location(request.Latitude, request.Longitude));

            // Save user
            var savedTracking = await _unitOfWork.TrackingRepository.SaveAsync(tracking);
            await _unitOfWork.CommitAsync(cancellationToken);

            _logger.LogInformation("[{className}] Order Tracking Created {savedTracking}", className, savedTracking.Id);
            return Result.Ok(true);
        }
        catch (Exception ex)
        {
            _logger.LogError("[{className}] Error creating Order Tracking :{request} Error: {ex}", className, request, ex);
            throw;
        }

    }
}
