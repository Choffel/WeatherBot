using Weather_Bot.Contract.Iss;
using Weather_Bot.DTOs.IssDTO;
using Weather_Bot.Redis;

namespace Weather_Bot.Service;

public class SatelliteStateService : ISatelliteStateService
{
    private readonly ICacheService _cacheService;
    
    private const string CacheKey = "satellite:iss:current";
    
    public SatelliteStateService(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }
    
    public async Task<SatelliteStateResponse> GetSatelliteStateAsync(CancellationToken cancellationToken)
    {
        var response = await _cacheService.GetAsync<SatelliteStateResponse>(CacheKey, cancellationToken);

        if (response == null)
        {
            throw new Exception("Failed to retrieve satellite state from cache.(в сервисе  пусто)");
        }
        
        return response;
    }
}