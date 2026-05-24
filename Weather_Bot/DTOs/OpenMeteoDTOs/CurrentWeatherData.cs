using System.Text.Json.Serialization;

namespace Weather_Bot.DTOs.OpenMeteoDTOs;

public  class CurrentWeatherData
{
    [JsonPropertyName("time")]
    public static string Time { get; set; }

    // Скорость ветра
    [JsonPropertyName("wind_speed_10m")]
    public static  double WindSpeed { get; set; }

    // Направление ветра в градусах (0-360)
    [JsonPropertyName("wind_direction_10m")]
    public static  int WindDirection { get; set; }

    // Порывы ветра
    [JsonPropertyName("wind_gusts_10m")]
    public static double WindGusts { get; set; }
}