using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.User.Contracts;
using Application.Shared.Abstractions;
using Domain.Features.Users.Entities;
using Domain.Features.Users.Repository;
using FluentResults;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using DomainUser = Domain.Features.Users.Entities.User;

namespace Application.Features.User.Commands.CreateUser;

public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, UserResponse>
{
    private readonly IUserRepository _repository;
    private readonly ILogger<CreateUserCommandHandler> _logger;
    private readonly IValidator<CreateUserCommand> _validator;
    private readonly ISecurityExtensions _securityExtensions;

    public CreateUserCommandHandler(
        ILogger<CreateUserCommandHandler> logger,
        IUserRepository repository,
        IValidator<CreateUserCommand> validator,
        ISecurityExtensions securityExtensions)
    {
        _repository = repository;
        _logger = logger;
        _validator = validator;
        _securityExtensions = securityExtensions;
    }

    public async Task<Result<UserResponse>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Validando usuário {request.Email}");

        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid) return Result.Fail(validationResult.Errors.Select(x => x.ErrorMessage));

        _logger.LogInformation($"Criando usuário {request.Email}");

        // Hash password to save
        var user = DomainUser.CreateUser
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
        return Result.Ok((UserResponse)savedUser);
    }
}
