using Application.Features.Plans.Contracts;
using Application.Shared.Abstractions;
using Domain.Features.Plans.Repository;
using Domain.Plans.Enums;
using Domain.Shared.Abstractions;
using FluentResults;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Application.Features.Plans.Commands.Update;

public class UpdatePlanCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<UpdatePlanCommandHandler> logger,
        IValidator<UpdatePlanCommand> validator) : ICommandHandler<UpdatePlanCommand, PlanResponse>
{

    public async Task<Result<PlanResponse>> Handle(UpdatePlanCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Atualizando plano {request.Name}");

        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid) return Result.Fail(validationResult.Errors.Select(x => x.ErrorMessage));

        var planRepository = unitOfWork.PlanRepository();

        var plan = await planRepository.GetByIdAsync(request.Id);
        if (plan == null) return Result.Fail("Plan not found");

        var pl = await planRepository.GetPlanByNameOrAmount(request.Name, request.Amount);
        if (pl != null && pl.Id != request.Id) return Result.Fail("A plan with this name or amount allready exists");


        var status = request.Status != 0 ? (PlanStatus)request.Status : plan.Status;
        plan.SetUpdatedAt(DateTime.Now);
        plan.SetAmount(request.Amount);
        plan.SetName(request.Name);
        plan.SetDescription(request.Description);
        plan.SetStatus(status);

        var savedPlan = await planRepository.UpdateAsync(plan);

        await unitOfWork.CommitAsync(cancellationToken);

        logger.LogInformation($"Plano {request.Name} atualizado com sucesso");
        return Result.Ok((PlanResponse)savedPlan);
    }
}