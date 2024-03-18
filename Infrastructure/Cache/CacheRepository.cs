using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;
using System.Threading.Tasks;
using Application.Shared.Abstractions;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace Infrastructure.Cache;

public class CacheRepository : ICacheRepository
{
    private readonly IConfiguration _configuration;
    private readonly ConnectionMultiplexer _redis;
    private readonly Lazy<ConnectionMultiplexer> LazyConnection;
    private readonly IDatabaseAsync _redisDB;
    private readonly int _rangeInKM;

    public CacheRepository(IConfiguration configuration)
    {
        _configuration = configuration;


        _rangeInKM = int.Parse(_configuration.GetSection("Configurations:RangeInKM").Value!);

        var configurationOptions = new ConfigurationOptions
        {
            EndPoints = { _configuration.GetSection("Infrastructure:Redis:Server").Value! }
        };
        LazyConnection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(configurationOptions));

        _redis = LazyConnection.Value;
        _redisDB = _redis.GetDatabase();
    }

    public async Task<bool> ExistsAsync(string key)
    {
        return await _redisDB.KeyExistsAsync(key);
    }

    public async Task<T?> GetAsync<T>(string key)
        where T : class?
    {
        var value = await _redisDB.StringGetAsync(key);
        if (value.HasValue) return JsonSerializer.Deserialize<T>(value!)!;
        return default(T?);
    }

    public async Task<string> SetAsync<T>(string key, T value, TimeSpan? expiry)
        where T : class
    {
        var serialized = JsonSerializer.Serialize<T>(value);
        await _redisDB.StringSetAsync(key, serialized, expiry);
        return serialized;
    }

    public async Task GeoAdd<T>(T value, double longitude, double latitude, string key)
        where T : class
    {
        await RemoveAsync(key);
        await SetAsync<T>(key, value, TimeSpan.FromMinutes(5));
        await _redisDB.GeoAddAsync("orders", new GeoEntry(longitude, latitude, key));
    }

    public async Task<List<T?>> GetNearOrders<T>(double longitude, double latitude)
        where T : class?
    {
        var results = await _redisDB.GeoRadiusAsync(
            "orders",
            longitude,
            latitude,
             _rangeInKM,
            GeoUnit.Kilometers, -1,
            Order.Ascending,
            GeoRadiusOptions.WithCoordinates);

        if (results is null || results.Length == 0) return [];

        List<T?> items = [];


        var tasks = results.ToList().Select(i => GetAsync<T>(i.Member.ToString()));
        var getAssyncResults = await Task.WhenAll(tasks);
        getAssyncResults.ToList().ForEach(x => items.Add(x));

        return items;
    }

    public async Task RemoveAsync(string key)
    {
        await _redisDB.GeoRemoveAsync("orders", key);
        await _redisDB.KeyDeleteAsync(key);
    }
}
