using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Features.Orders.Contracts;

public record OrderUserResponse(long Id, string Name, string Email, string Document, string? Photo);
