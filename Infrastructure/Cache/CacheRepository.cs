using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Application.Shared.Abstractions;
using Application.Shared.Extensions;
using Domain.Shared.Abstractions;
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
    private readonly JsonSerializerOptions _serializationOptions = new()
    {
        ReferenceHandler = ReferenceHandler.IgnoreCycles
    };

    private readonly string _geoContainer = "orders";

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
        var serialized = JsonSerializer.Serialize<T>(value, _serializationOptions);
        await _redisDB.StringSetAsync(key, serialized, expiry);
        return serialized;
    }

    public async Task GeoAdd<T>(List<T> values)
    where T : class, IGeoData
    {
        foreach (var value in values) await GeoAdd(value, KeyExtensions.OrderUserKey(value.UserId));
    }

    public async Task GeoAdd<T>(T value, string? key = null)
        where T : class, IGeoData
    {
        key ??= KeyExtensions.OrderUserKey(value.UserId);

        await RemoveAsync(key);
        await SetAsync<T>(key, value, TimeSpan.FromMinutes(5));

        var geoEntry = new GeoEntry(value.Location.Longitude, value.Location.Latitude, key);
        await _redisDB.GeoAddAsync(_geoContainer, geoEntry);
    }

    public async Task<List<T?>> GetNearObjects<T>(double longitude, double latitude)
        where T : class?
    {
        var results = await _redisDB.GeoRadiusAsync(
            _geoContainer,
            longitude,
            latitude,
            _rangeInKM,
            GeoUnit.Kilometers, -1,
            Order.Ascending,
            GeoRadiusOptions.WithCoordinates);

        if (results is null || results.Length == 0) return [];

        List<T?> items = [];

        var tasks = results.ToList().Select(async i =>
        {
            var cachedObject = await GetAsync<T>(i.Member.ToString());

            if (cachedObject is not null)
            {
                items.Add(cachedObject);
                return items;
            }

            await RemoveAsync(i.Member.ToString());
            return items;
        });

        await Task.WhenAll(tasks);

        return items;
    }

    public async Task RemoveAsync(string key)
    {
        await _redisDB.GeoRemoveAsync(_geoContainer, key);
        await _redisDB.KeyDeleteAsync(key);
    }
}
