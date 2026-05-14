using Weather_Bot.DTOs;

namespace Weather_Bot.Contract;

public interface IWeatherData
{
    Task<WindyWeatherResponse?> GetRawForecastAsync(double lat, double lon);
    
    // Обработка данных: например, поиск максимального ветра на ближайшие 24 часа
    Task<string> GetDrakeSummaryAsync();
}