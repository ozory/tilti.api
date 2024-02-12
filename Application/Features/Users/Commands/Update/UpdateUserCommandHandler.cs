using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Users.Contracts;
using Application.Shared.Abstractions;
using Domain.Enums;
using Domain.Features.Users.Entities;
using Domain.Features.Users.Repository;
using FluentResults;
using FluentValidation;
using Microsoft.Extensions.Logging;
using DomainUser = Domain.Features.Users.Entities.User;

namespace Application.Features.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler : ICommandHandler<UpdateUserCommand, UserResponse>
{
    private readonly IUserRepository _repository;
    private readonly ILogger<UpdateUserCommandHandler> _logger;
    private readonly IValidator<UpdateUserCommand> _validator;
    private readonly ISecurityExtensions _securityExtensions;

    public UpdateUserCommandHandler(
        ILogger<UpdateUserCommandHandler> logger,
        IUserRepository repository,
        IValidator<UpdateUserCommand> validator,
        ISecurityExtensions securityExtensions)
    {
        _repository = repository;
        _logger = logger;
        _validator = validator;
        _securityExtensions = securityExtensions;
    }

    public async Task<Result<UserResponse>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Validando usuário {request.Email}");

        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid) return Result.Fail(validationResult.Errors.Select(x => x.ErrorMessage));

        var user = await _repository.GetByIdAsync(request.id);
        if (user == null) return Result.Fail("User not found");

        user.SetName(request.Name);
        user.SetEmail(request.Email);
        user.SetDocument(request.Document);

        if (request.Status is not null && (ushort)user.Status != request.Status)
            user.SetStatus((UserStatus)request.Status);

        // Generate Hashed Password
        if (!string.IsNullOrEmpty(request.Password))
        {
            user.SetVerificationSalt(_securityExtensions.GenerateSalt());
            user.SetPassword(_securityExtensions.ComputeHash(user.VerificationSalt!, request.Password));
        }

        _logger.LogInformation($"Atualizando usuário {request.Email}");
        var savedUser = await _repository.UpdateAsync(user);

        _logger.LogInformation($"Usuário {request.Email} atualizado com sucesso");
        return Result.Ok((UserResponse)savedUser);
    }
}
