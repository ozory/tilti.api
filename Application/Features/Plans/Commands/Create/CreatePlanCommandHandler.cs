using Application.Features.Plans.Contracts;
using Application.Shared.Abstractions;
using Domain.Features.Plans.Entities;
using Domain.Features.Plans.Repository;
using Domain.Plans.Enums;
using Domain.Shared.Abstractions;
using FluentResults;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Application.Features.Plans.Commands.Create;

public class CreatePlanCommandHandler(
    IUnitOfWork unitOfWork,
    ILogger<CreatePlanCommandHandler> logger,
    IValidator<CreatePlanCommand> validator) : ICommandHandler<CreatePlanCommand, PlanResponse>
{

    public async Task<Result<PlanResponse>> Handle(CreatePlanCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Criando plano {request.Name}");

        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid) return Result.Fail(validationResult.Errors.Select(x => x.ErrorMessage));

        var planRepository = unitOfWork.GetPlanRepository();

        logger.LogInformation($"Verificando se plano {request.Name}");
        var pl = await planRepository.GetPlanByNameOrAmount(request.Name, request.Amount);
        if (pl != null) return Result.Fail("A plan with this name or amount allready exists");

        var plan = Plan.Create(
            null,
            request.Name,
            request.Description,
            request.Amount,
            (ushort)PlanStatus.Active,
            DateTime.Now
        );

        var savedPlan = await planRepository.SaveAsync(plan);
        await unitOfWork.CommitAsync(cancellationToken);

        return Result.Ok((PlanResponse)savedPlan);
    }
}