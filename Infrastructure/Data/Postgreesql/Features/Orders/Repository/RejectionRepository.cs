using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Shared.Abstractions;
using Domain.Enums;
using Domain.Features.Orders.Entities;
using Domain.Features.Orders.Repository;
using Domain.Features.Users.Entities;
using Infrastructure.Data.Postgreesql.Shared;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Data.Postgreesql.Features.Orders.Repository;

public class RejectionRepository :
    GenericRepository<Rejection>,
    IRejectRepository
{
    private readonly ICacheRepository _cacheRepository;

    private readonly IConfiguration _configuration;

    public RejectionRepository(
        TILTContext context,
        ICacheRepository cacheRepository,
        IConfiguration configuration) : base(context)
    {
        _cacheRepository = cacheRepository;
        _configuration = configuration;
    }

    private readonly string IncludeProperties = $"{nameof(Order)}";

    public async Task<IReadOnlyList<Rejection?>> GetRejectionsByUser(long driverId)
    {
        var _minutes = int.Parse(_configuration["Configurations:RejectionMinutes"]!);
        var date = DateTime.Now.AddMinutes(_minutes);

        var rejections = await Filter(
            filter: s => s.DriverId == driverId
            && s.CreatedAt >= date && s.Order.Status.Equals(OrderStatus.ReadyToAccept)
            , includeProperties: IncludeProperties);

        return rejections;
    }

}
