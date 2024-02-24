using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Shared.Abstractions;
using Domain.Features.Orders.Entities;
using Domain.ValueObjects;
using GoogleApi.Entities.Common;
using GoogleApi.Entities.Maps.Common;
using GoogleApi.Entities.Maps.Directions.Request;
using GoogleApi.Entities.Maps.Directions.Response;
using Address = GoogleApi.Entities.Common.Address;
using RequestWayPoint = GoogleApi.Entities.Maps.Directions.Request.WayPoint;
using DirectionsResponse = GoogleApi.Entities.Maps.Directions.Response.DirectionsResponse;
using GoogleApi.Entities.Maps.Common.Enums;
using System.Text;
using GoogleApi.Entities.Maps.DistanceMatrix.Request;
using Microsoft.Extensions.Caching.Distributed;
using GoogleApi.Entities.Maps.DistanceMatrix.Response;
using System.Text.Json;

namespace Infrastructure.Features.Orders.Services;

public class MapServices : IMapServices
{
    private readonly decimal pricePerKM;
    private readonly decimal pricePerDuration;
    private readonly string apiKey;
    private readonly IDistributedCache _distributedCache;

    public MapServices(
        decimal pricePerKM,
        decimal pricePerDuration,
        string apiKey,
        IDistributedCache distributedCache)
    {
        this.apiKey = apiKey;
        this.pricePerKM = pricePerKM;
        this.pricePerDuration = pricePerDuration;
        _distributedCache = distributedCache;
    }

    public decimal PricePerKM { get; }

    public async Task<Order> CalculateOrderAsync(Order order)
    {
        var orderAddress = order.Addresses.OrderBy(x => x.AddressType).ToList();

        DistanceMatrixRequest dm = new DistanceMatrixRequest();
        dm.Key = apiKey;
        dm.TravelMode = TravelMode.Driving;
        dm.DepartureTime = DateTime.UtcNow;

        dm.Origins = orderAddress.SkipLast(1).Select(x => new LocationEx(new Address(x.Street))).ToList();
        var Destination = orderAddress.Last();
        dm.Destinations = new List<LocationEx> { new LocationEx(new Address(Destination.Street)) };

        DistanceMatrixResponse distanceMatrixResponse = await GetDisanceMatrix(dm);
        ApplyValues(order, distanceMatrixResponse);

        return order;
    }

    private async Task<DistanceMatrixResponse> GetDisanceMatrix(DistanceMatrixRequest dm)
    {
        // Cria o token e busca do Cache
        StringBuilder sb = new StringBuilder();

        var origins = dm.Origins.Select(x => x.String);
        var destnations = dm.Destinations.Select(x => x.String);

        sb.Append(origins.Aggregate(func: (result, item) => result + item));
        sb.Append('|');
        sb.Append(destnations.Aggregate(func: (result, item) => result + item));

        var cacheKey = sb.ToString().Replace(" ", string.Empty);
        var cacheResponse = await _distributedCache.GetStringAsync(cacheKey);

        if (cacheResponse is not null)
            return JsonSerializer.Deserialize<DistanceMatrixResponse>(cacheResponse)!;


        return await CallGoogleApiAndSetCache(dm, cacheKey);
    }

    private async Task<DistanceMatrixResponse> CallGoogleApiAndSetCache(DistanceMatrixRequest dm, string cacheKey)
    {
        DistanceMatrixResponse distanceMatrixResponse = await GoogleApi.GoogleMaps.DistanceMatrix.QueryAsync(dm);

        string distanceMatrix = JsonSerializer.Serialize(distanceMatrixResponse);
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
            SlidingExpiration = TimeSpan.FromMinutes(3)
        };

        await _distributedCache.SetStringAsync(cacheKey, distanceMatrix, options);
        return distanceMatrixResponse;
    }

    private void ApplyValues(Order order, DistanceMatrixResponse distanceMatrixResponse)
    {
        var result = distanceMatrixResponse.Rows.First().Elements.First();
        var distance = result.Distance.Value;
        var duration = result.Duration.Value;

        order.SetDistanceInKM(distance);
        order.SetDurationInSeconds(duration);

        var valuePerKM = (pricePerKM * order.DistanceInKM) / 1000;
        var valuePerDuration = (pricePerDuration * (order.DurationInSeconds / 60));

        order.SetAmount(valuePerKM + valuePerDuration);
    }
}
