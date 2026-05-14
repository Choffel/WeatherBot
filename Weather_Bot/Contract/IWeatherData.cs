using Weather_Bot.DTOs;
using Weather_Bot.Enum;

namespace Weather_Bot.Contract;

public interface IWeatherData
{
    Task<WindyWeatherResponse?> GetRawForecastAsync(double lat, double lon);

    Task<string> GetWeatherSummaryAsync(WeatherReportType type);
}