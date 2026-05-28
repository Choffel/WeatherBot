using Weather_Bot.DTOs.OpenMeteoDTOs;

namespace Weather_Bot.Contract.NemoPoint;

public interface INemoPoint
{
    Task<OpenMeteoResponse> GetNemoPointWeatherAsync();
}