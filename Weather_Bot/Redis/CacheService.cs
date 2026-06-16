using System.Text.Json;
using StackExchange.Redis;

namespace Weather_Bot.Redis;

public class CacheService : ICacheService
{
    private readonly IDatabase _database;
    
    private const string CacheKeyPrefix = "OrbitTrackerCache:";
    
    public CacheService(IConnectionMultiplexer redis)
    {
        _database = redis.GetDatabase();
    }
    
    public  async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var value = await _database.StringGetAsync(CacheKeyPrefix + key);
        if (!value.HasValue)
        {
            return default;
        }
        
        return JsonSerializer.Deserialize<T>((string)value!, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }
}