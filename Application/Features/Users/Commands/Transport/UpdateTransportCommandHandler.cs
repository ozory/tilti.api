using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Users.Commands.CreateUser;
using Application.Features.Users.Contracts;
using Application.Shared.Abstractions;
using Domain.Shared.Abstractions;
using FluentResults;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Application.Features.Users.Commands.Transport;

public class UpdateTransportCommandHandler(
    IUnitOfWork unitOfWork,
    IValidator<UpdateTransportCommand> validator,
    ILogger<UpdateTransportCommandHandler> logger) : ICommandHandler<UpdateTransportCommand, UserResponse>
{
    private readonly string className = nameof(UpdateTransportCommandHandler);

    public async Task<Result<UserResponse>> Handle(UpdateTransportCommand request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("[{className}] Validando transporte {plate}", className, request.Plate);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid) return Result.Fail(validationResult.Errors.Select(x => x.ErrorMessage));

            var user = await unitOfWork.UserRepository.GetByIdAsync(request.UserId);
            if (user is null) return Result.Fail("Usuário não encontrado");

            user.SetTransport(
                request.Name,
                request.Description,
                request.Year,
                request.Model,
                request.Plate
            );

            logger.LogInformation("[{className}] Atualizando usuário {Email}", className, user.Email);
            var savedUser = await unitOfWork.UserRepository.UpdateAsync(user);
            await unitOfWork.CommitAsync(cancellationToken);

            logger.LogInformation("[{className}] Usuário {Email} atualizado com sucesso", className, user.Email);
            return Result.Ok((UserResponse)savedUser);
        }
        catch (Exception ex)
        {
            logger.LogError("[{className}] Error updating User :{request} Error: {ex}", className, request, ex);
            throw;
        }
    }
}
