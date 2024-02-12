using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Security.Entities;
using Application.Features.Users.Contracts;
using Application.Shared.Abstractions;
using Domain.Features.Users.Entities;
using Domain.Features.Users.Repository;
using FluentResults;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Features.Security.Commands.Authenticate;

public class AuthenticateUserCommandHandler
    : ICommandHandler<AuthenticateUserCommand, AuthenticationResponse>
{
    private readonly IUserRepository _repository;
    private readonly ILogger<AuthenticateUserCommandHandler> _logger;
    private readonly IValidator<AuthenticateUserCommand> _validator;
    private readonly ISecurityExtensions _securityExtensions;
    private readonly ISecurityRepository _securityRepository;

    public AuthenticateUserCommandHandler(
        ILogger<AuthenticateUserCommandHandler> logger,
        IUserRepository repository,
        IValidator<AuthenticateUserCommand> validator,
        ISecurityExtensions securityExtensions,
        ISecurityRepository securityRepository)
    {
        _repository = repository;
        _logger = logger;
        _validator = validator;
        _securityExtensions = securityExtensions;
        _securityRepository = securityRepository;
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

        RefreshTokens refreshToken = new()
        {
            UserId = user!.Id,
            RefreshToken = _securityExtensions.GenerateRefreshToken(),
            LastLogin = DateTime.Now
        };

        var authenticateResponse = new AuthenticationResponse(
            user.Id,
            user.Name.Value,
            user.Email.Value,
            _securityExtensions.GenerateToken(user),
            refreshToken.RefreshToken);

        await _securityRepository.DeleteToken(user.Id);
        await _securityRepository.SaveToken(refreshToken);

        return Result.Ok(authenticateResponse);
    }
}
