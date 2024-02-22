using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Plans.Contracts;
using Application.Shared.Abstractions;
using Domain.Features.Plans.Entities;
using Domain.Features.Plans.Repository;
using Domain.Plans.Enums;
using FluentResults;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Application.Features.Plans.Commands.Create;

public class CreatePlanCommandHandler : ICommandHandler<CreatePlanCommand, PlanResponse>
{
    private readonly IPlanRepository _repository;
    private readonly ILogger<CreatePlanCommandHandler> _logger;
    private readonly IValidator<CreatePlanCommand> _validator;

    public CreatePlanCommandHandler(
        IPlanRepository repository,
        ILogger<CreatePlanCommandHandler> logger,
        IValidator<CreatePlanCommand> validator)
    {
        _repository = repository;
        _logger = logger;
        _validator = validator;
    }

    public async Task<Result<PlanResponse>> Handle(CreatePlanCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Criando plano {request.Name}");

        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid) return Result.Fail(validationResult.Errors.Select(x => x.ErrorMessage));

        var pl = await _repository.GetPlanByNameOrAmount(request.Name, request.Amount);
        if (pl != null) return Result.Fail("A plan with this name or amount allready exists");

        var plan = Plan.Create(
            null,
            request.Name,
            request.Description,
            request.Amount,
            (ushort)PlanStatus.Active,
            DateTime.Now
        );

        var savedPlan = await _repository.SaveAsync(plan);
        return Result.Ok((PlanResponse)savedPlan);
    }
}