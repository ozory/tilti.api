using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Orders.Entities;

namespace Application.Shared.Abstractions
{
    public interface IMapServices
    {
        Task<Order> CalculateOrderAsync(Order order);
    }
}