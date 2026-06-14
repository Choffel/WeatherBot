using Weather_Bot.DTOs.OpenMeteoDTOs;

namespace Weather_Bot.Formatters;

/// <summary>
/// Отвечает за форматирование сообщений о погоде в HTML.
/// Single Responsibility: только форматирование текста.
/// </summary>
public class WeatherMessageFormatter
{
    /// <summary>
    /// Форматирует сообщение о ветре (базовые координаты)
    /// </summary>
    public string FormatWindMessage(CurrentWeatherData current)
    {
        return $"💨 <b>Скорость ветра:</b> {current.WindSpeed} km/h\n" +
               $"🌬 <b>Порывы:</b> {current.WindGusts} km/h\n" +
               $"🧭 <b>Направление:</b> {current.WindDirection}°\n" +
               $"🕒 <b>Время:</b> {current.Time}";
    }

    /// <summary>
    /// Форматирует сообщение о ветре и температуре в Люблине
    /// </summary>
    public string FormatLublinWeatherMessage(CurrentWeatherData current)
    {
        return $"💨 <b>Скорость ветра:</b> {current.WindSpeed} km/h\n" +
               $"🌬 <b>Порывы:</b> {current.WindGusts} km/h\n" +
               $"🌡️ <b>Температура:</b> {current.Temperature}°C\n" +
               $"🧭 <b>Направление:</b> {current.WindDirection}°\n" +
               $"🕒 <b>Время:</b> {current.Time}";
    }

    /// <summary>
    /// Форматирует сообщение о погоде прямо под МКС с координатами
    /// </summary>
    public string FormatIssWeatherMessage(CurrentWeatherData current, double latitude, double longitude)
    {
        return $"🛰 <b>Сводка погоды ПРЯМО ПОД МКС:</b>\n\n" +
               $"📍 <b>Координаты:</b> {latitude:F4}, {longitude:F4}\n" +
               $"💨 <b>Скорость ветра:</b> {current.WindSpeed} km/h\n" +
               $"🔹 <b>Порывы ветра:</b> {current.WindGusts} km/h\n" +
               $"🧭 <b>Направление:</b> {current.WindDirection}°\n" +
               $"🕒 <b>Время замера погоды:</b> {current.Time}";
    }

    /// <summary>
    /// Форматирует сообщение о погоде по координатам (для расширенного вывода)
    /// </summary>
    public string FormatWeatherWithCoordinatesMessage(
        CurrentWeatherData current,
        double latitude,
        double longitude,
        string? location = null)
    {
        var header = string.IsNullOrEmpty(location)
            ? $"💨 <b>Сводка погоды по координатам:</b> {latitude:F2}, {longitude:F2}\n\n"
            : $"💨 <b>Сводка погоды в {location}:</b>\n\n";

        return header +
               $"🔹 <b>Скорость ветра:</b> {current.WindSpeed} km/h\n" +
               $"🔹 <b>Порывы ветра:</b> {current.WindGusts} km/h\n" +
               $"🧭 <b>Направление:</b> {current.WindDirection}°\n" +
               (current.Temperature.HasValue
                   ? $"🌡️ <b>Температура:</b> {current.Temperature}°C\n"
                   : "") +
               $"🕒 <b>Время замера:</b> {current.Time}";
    }
}

