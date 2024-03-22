using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Shared.Abstractions;
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

    Task GeoAdd<T>(T value, string? key = null)
        where T : class, IGeoData;

    Task GeoAdd<T>(List<T> values)
        where T : class, IGeoData;

    Task<List<T?>> GetNearObjects<T>(double longitude, double latitude)
        where T : class?;
}
