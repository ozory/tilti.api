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

    private readonly string className = nameof(CreatePlanCommandHandler);

    public async Task<Result<PlanResponse>> Handle(CreatePlanCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("[{className}] Criando plano {plan}", className, request.Name);
        try
        {
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid) return Result.Fail(validationResult.Errors.Select(x => x.ErrorMessage));

            logger.LogInformation("[{className}] Verificando se {plan}", className, request.Name);

            var pl = await unitOfWork.PlanRepository.GetPlanByNameOrAmount(request.Name, request.Amount);
            if (pl != null) return Result.Fail("A plan with this name or amount allready exists");

            var plan = Plan.Create(
                null,
                request.Name,
                request.Description,
                request.Amount,
                (ushort)PlanStatus.Active,
                DateTime.Now
            );

            var savedPlan = await unitOfWork.PlanRepository.SaveAsync(plan);
            await unitOfWork.CommitAsync(cancellationToken);

            logger.LogInformation("[{className}] Plan {plan} created", className, request.Name);
            return Result.Ok((PlanResponse)savedPlan);
        }
        catch (Exception ex)
        {
            logger.LogError("[{className}] Error Creating Plan : {request} Error: {ex}", className, request, ex);
            throw;
        }
    }
}