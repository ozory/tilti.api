using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Users.Contracts;
using Application.Shared.Abstractions;

namespace Application.Features.Users.Commands.Transport;

public sealed record UpdateTransportCommand(
    long UserId,
    string Name,
    string Description,
    ushort Year,
    string Plate,
    string Model
) : ICommand<UserResponse>;

