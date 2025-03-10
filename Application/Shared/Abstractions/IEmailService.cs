using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Users.Events;

namespace Application.Shared.Abstractions;

public interface IEmailService
{
    Task SendConfirmationEmail(UserCreatedDomainEvent user);
}
