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

namespace Application.Features.Orders.Services;

public class MapServices : IMapServices
{
    public async Task<Order> CalculateOrderAsync(Order order)
    {
        DirectionsRequest request = new DirectionsRequest();

        request.Key = "AIzaSyAJgBs8LYok3rt15rZUg4aUxYIAYyFzNcw";

        var orderAddress = order.Addresses.OrderBy(x => x.AddressType).ToList();

        foreach (var item in orderAddress)
        {
            Address address = new Address(item.Street);
            LocationEx location = new LocationEx(address);
            RequestWayPoint wayPoint = new RequestWayPoint(location);

            request.WayPoints.Append(wayPoint);
        }

        DirectionsResponse response = await GoogleApi.GoogleMaps.Directions.QueryAsync(request);

        var routes = response.Routes.FirstOrDefault();
        var distance = routes!.Legs.First().Distance.Text;

        throw new NotImplementedException();
    }
}
