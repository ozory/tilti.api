using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Plans.Commands.Update;
using Application.Features.Plans.Contracts;
using Application.Shared.Abstractions;
using Domain.Features.Plans.Repository;
using FluentResults;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Application.Features.Plans.Commands.Update;

public class UpdatePlanCommandHandler : ICommandHandler<UpdatePlanCommand, PlanResponse>
{
    private readonly IPlanRepository _repository;
    private readonly ILogger<UpdatePlanCommandHandler> _logger;
    private readonly IValidator<UpdatePlanCommand> _validator;

    public UpdatePlanCommandHandler(
        IPlanRepository repository,
        ILogger<UpdatePlanCommandHandler> logger,
        IValidator<UpdatePlanCommand> validator)
    {
        _repository = repository;
        _logger = logger;
        _validator = validator;
    }

    public async Task<Result<PlanResponse>> Handle(UpdatePlanCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Criando plano {request.Name}");

        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid) return Result.Fail(validationResult.Errors.Select(x => x.ErrorMessage));

        var pl = await _repository.GetPlanByNameOrAmount(request.Name, request.Amount);
        if (pl != null && pl.Id != request.Id) return Result.Fail("A plan with this name or amount allready exists");

        var plan = await _repository.GetByIdAsync(request.Id);
        if (plan == null) return Result.Fail("Plan not found");

        var savedPlan = await _repository.UpdateAsync(plan);
        return Result.Ok((PlanResponse)savedPlan);
    }
}