using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Shared.Abstractions;
using Domain.Entities;
using Domain.Features.Subscription.Entities;
using Domain.Features.Subscription.Repository;
using Domain.Features.User.Repository;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Subscriptions.Commands.CreateSubscription;

public class CreateSubscriptionCommandHandler : ICommandHandler<CreateSubscriptionCommand, Subscription>
{
    private readonly ISubscriptionRepository _repository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<CreateSubscriptionCommandHandler> _logger;

    public CreateSubscriptionCommandHandler(
        ILogger<CreateSubscriptionCommandHandler> logger,
        ISubscriptionRepository repository,
        IUserRepository userRepository)
    {
        _repository = repository;
        _logger = logger;
        _userRepository = userRepository;
    }

    public async Task<Result<Subscription>> Handle(CreateSubscriptionCommand request, CancellationToken cancellationToken)
    {
        var result = new Result<Subscription>();
        // Validações: 
        //   - Verificar se o plano existe e está ativo
        //   - Verificar se o usuario existe e está ativo
        //   - Verificar se o usuário já não tem uma assinatura vigente

        // TODO: Implementar essas validações usando FluentValidation, usando Polly\

        var user = await _userRepository.GetByIdAsync(request.userId);
        var createResult = Subscription.Create(user!, new Domain.Plans.Entities.Plan());

        return createResult;
    }

}
