using Domain.Features.Orders.Entities;

namespace Application.Shared.Abstractions
{
    public interface IMapServices
    {
        Task<Order> CalculateOrderAsync(Order order);
    }
}