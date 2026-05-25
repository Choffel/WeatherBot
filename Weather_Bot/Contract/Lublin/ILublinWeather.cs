using Weather_Bot.DTOs.OpenMeteoDTOs;

namespace Weather_Bot.Contract.Lublin;

public interface ILublinWeather
{
    Task<OpenMeteoResponse> GetWindAndTempAsync();
}