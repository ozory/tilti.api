using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.User.Contracts;
using Application.Features.User.Extensions;
using Application.Shared.Abstractions;
using Domain.Features.User.Entities;
using Domain.Features.User.Repository;
using FluentResults;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using DomainUser = Domain.Features.User.Entities.User;

namespace Application.Features.User.Commands.CreateUser;

public class AuthenticateUserCommandHandler
    : ICommandHandler<AuthenticateUserCommand, AuthenticationResponse>
{
    private readonly IUserRepository _repository;
    private readonly ILogger<CreateUserCommandHandler> _logger;
    private readonly IValidator<AuthenticateUserCommand> _validator;
    private readonly ISecurityExtensions _securityExtensions;

    public AuthenticateUserCommandHandler(
        ILogger<CreateUserCommandHandler> logger,
        IUserRepository repository,
        IValidator<AuthenticateUserCommand> validator,
        ISecurityExtensions securityExtensions)
    {
        _repository = repository;
        _logger = logger;
        _validator = validator;
        _securityExtensions = securityExtensions;
    }

    public async Task<Result<AuthenticationResponse>> Handle(
        AuthenticateUserCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Validando usuário {request.Email}");

        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid) return Result.Fail(validationResult.Errors.Select(x => x.ErrorMessage));

        _logger.LogInformation($"Criando usuário {request.Email}");
        var user = await _repository.GetByEmail(request.Email);

        var passwordHash = _securityExtensions.ComputeHash(user!.VerificationSalt!, request.Password);

        if (user.Password!.Value != passwordHash)
        {
            _logger.LogWarning($"Falha de login {request.Email}");
            return Result.Fail("User not found");
        }

        _logger.LogInformation($"Usuário {request.Email} logado com sucesso");
        var authenticateResponse = new AuthenticationResponse(
            user.Id,
            user.Name.Value,
            user.Email.Value,
            _securityExtensions.GenerateToken(user));

        return Result.Ok(authenticateResponse);
    }
}
