using Application.Interfaces;
using StackExchange.Redis;
using System.Text.Json;
using Polly;
using Polly.CircuitBreaker;

namespace Infrastructure.Caching;

public class RedisCacheService : ICacheService
{
    private readonly IDatabase _cache;
    private readonly AsyncPolicy _redisPolicy;

    public RedisCacheService(
        IConnectionMultiplexer redis)
    {
        _cache = redis.GetDatabase();

        var retryPolicy = Policy
            .Handle<RedisConnectionException>()
            .Or<RedisTimeoutException>()
            .WaitAndRetryAsync(
                3,
                retryAttempt =>
                    TimeSpan.FromSeconds(
                        Math.Pow(2, retryAttempt)),
                (exception, delay, retryCount, context) =>
                {
                    Console.WriteLine(
                        $"Redis retry {retryCount}: {exception.Message}");
                });

        var circuitBreaker = Policy
            .Handle<RedisConnectionException>()
            .Or<RedisTimeoutException>()
            .CircuitBreakerAsync(
                5,
                TimeSpan.FromSeconds(30));

        _redisPolicy = Policy.WrapAsync(
            retryPolicy,
            circuitBreaker);
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        try
        {
            return await _redisPolicy.ExecuteAsync(async () =>
            {
                var value = await _cache.StringGetAsync(key);

                if (value.IsNullOrEmpty)
                    return default;

                return JsonSerializer.Deserialize<T>(value);
            });
        }
        catch
        {
            // Redis failed
            return default;
        }
    }

    public async Task SetAsync<T>(
        string key,
        T value,
        TimeSpan expiration)
    {
        await _redisPolicy.ExecuteAsync(async () =>
        {
            var json = JsonSerializer.Serialize(value);

            await _cache.StringSetAsync(
                key,
                json,
                expiration);
         
        });
    }

    public async Task RemoveAsync(string key)
    {
        await _cache.KeyDeleteAsync(key);
    }
}