using Weather_Bot.DTOs;
using Weather_Bot.Enum;

namespace Weather_Bot.Contract;

public interface IWeatherData
{
    Task<WindyWeatherResponse> GetWindAsync(double latitude, double longitude);
    
    Task<WindyWeatherResponse> GetWaveAsync(double latitude, double longitude);
}