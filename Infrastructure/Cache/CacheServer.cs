using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Cache;

public class CacheServer : ICacheServer
{

    private readonly IConfiguration _configuration;
    public CacheServer(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task GeoAdd(double longitude, double latitude, string name)
    {
        throw new NotImplementedException();
    }

    public Task<T> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        throw new NotImplementedException();
    }

    public Task<T> GetNearOrders<T>(string key, double longitude, double latitude)
    {
        throw new NotImplementedException();
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken cancellationToken = default) where T : class
    {
        throw new NotImplementedException();
    }
}
