using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Application.Shared.Abstractions;

public interface ICacheRepository
{
    Task<T?> GetAsync<T>(string key)
        where T : class?;

    Task<string> SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        where T : class;

    Task RemoveAsync(string key);

    Task<bool> ExistsAsync(string key);

    Task GeoAdd<T>(T value, double longitude, double latitude, string key)
        where T : class;

    Task<List<T?>> GetNearOrders<T>(double longitude, double latitude)
        where T : class?;
}
