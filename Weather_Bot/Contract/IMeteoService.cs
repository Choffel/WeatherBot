using Weather_Bot.DTOs.OpenMeteoDTOs;

namespace Weather_Bot.Contract;

public interface IMeteoService
{
    Task<OpenMeteoResponse?> GetWindAsync();
    
    Task<OpenMeteoResponse?> GetWeatherIssAsync(double latitude, double longitude);
}