using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Orders.Contracts;
using Application.Features.Users.Contracts;
using Application.Shared.Abstractions;
using DomainUser = Domain.Features.Users.Entities.User;
namespace Application.Features.Orders.Commands.UpdateOrder;

public sealed record UpdateOrderCommand
(
    long id,
    string Name,
    string Email,
    string Document,
    string? Password,
    ushort? Status

) : ICommand<OrderResponse>;


