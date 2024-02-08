using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Abstractions;
using Domain.ValueObjects;
using FluentResults;
using Domain.Features.Users.Entities;
namespace Domain.Features.Order.Entities;

public class Item : Entity<Item>
{
    public User? User { get; protected set; }
    public uint Quantity { get; protected set; }
    public Name Name { get; protected set; } = null!;
    public Amount Amount { get; protected set; } = null!;
    public Description Description { get; protected set; } = null!;
}
