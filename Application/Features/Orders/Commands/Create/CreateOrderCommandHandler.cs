using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Orders.Contracts;
using Application.Features.Users.Contracts;
using Application.Shared.Abstractions;
using Domain.Features.Users.Entities;
using Domain.Features.Users.Repository;
using FluentResults;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand, OrderResponse>
{
    private readonly IUserRepository _repository;
    private readonly ILogger<CreateOrderCommandHandler> _logger;
    private readonly IValidator<CreateOrderCommand> _validator;
    private readonly ISecurityExtensions _securityExtensions;

    public CreateOrderCommandHandler(
        ILogger<CreateOrderCommandHandler> logger,
        IUserRepository repository,
        IValidator<CreateOrderCommand> validator,
        ISecurityExtensions securityExtensions)
    {
        _repository = repository;
        _logger = logger;
        _validator = validator;
        _securityExtensions = securityExtensions;
    }

    public async Task<Result<OrderResponse>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Validando usuário {request.Email}");

        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid) return Result.Fail(validationResult.Errors.Select(x => x.ErrorMessage));

        _logger.LogInformation($"Criando usuário {request.Email}");

        // Hash password to save
        var user = User.CreateUser
            (
                null,
                request.Name,
                request.Email,
                request.Document,
                request.Password,
                DateTime.Now
            );

        // Security Info
        var securitySalt = _securityExtensions.GenerateSalt();
        user.SetVerificationSalt(securitySalt);
        user.SetPassword(_securityExtensions.ComputeHash(securitySalt, request.Password));
        user.SetVerificationCode(_securityExtensions.ComputeValidationCode());

        // Save user
        var savedUser = await _repository.SaveAsync(user);

        _logger.LogInformation($"Usuário {request.Email} criado com sucesso");
        return Result.Ok();
        // return Result.Ok((UserResponse)savedUser);
    }
}
