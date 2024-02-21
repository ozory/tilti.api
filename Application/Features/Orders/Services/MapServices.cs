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

namespace Application.Features.Orders.Services;

public class MapServices : IMapServices
{
    private readonly decimal pricePerKM;
    private readonly decimal pricePerDuration;
    private readonly string apiKey;

    public MapServices(decimal pricePerKM, decimal pricePerDuration, string apiKey)
    {
        this.apiKey = apiKey;
        this.pricePerKM = pricePerKM;
        this.pricePerDuration = pricePerDuration;
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

        var response = await GoogleApi.GoogleMaps.DistanceMatrix.QueryAsync(dm);

        var result = response.Rows.First().Elements.First();
        var distance = result.Distance.Value;
        var duration = result.Duration.Value;

        order.SetDistanceInKM(distance);
        order.SetDurationInSeconds(duration);

        var valuePerKM = (pricePerKM * order.DistanceInKM) / 1000;
        var valuePerDuration = (pricePerDuration * (order.DurationInSeconds / 60));

        order.SetAmount(valuePerKM + valuePerDuration);

        return order;
    }
}
