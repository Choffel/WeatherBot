using Weather_Bot.DTOs;
using Weather_Bot.Enum;

namespace Weather_Bot.Contract;

public interface IReportWeather
{
    string FormatReport(WindyWeatherResponse data, WeatherReportType type);

    double CalculateSpeed(object uObj, object vObj);
}