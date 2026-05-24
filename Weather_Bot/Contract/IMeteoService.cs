using Weather_Bot.DTOs.OpenMeteoDTOs;

namespace Weather_Bot.Contract;

public interface IMeteoService
{
    Task<OpenMeteoResponse?> GetWindAsync(double latitude, double longitude);
    
}