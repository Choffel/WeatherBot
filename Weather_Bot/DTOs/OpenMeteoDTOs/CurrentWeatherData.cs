using System.Text.Json.Serialization;

namespace Weather_Bot.DTOs.OpenMeteoDTOs;

public class CurrentWeatherData
{
    [JsonPropertyName("time")]
    public required string Time { get; set; }

    // Скорость ветра
    [JsonPropertyName("wind_speed_10m")]
    public double WindSpeed { get; set; }

    // Направление ветра в градусах (0-360)
    [JsonPropertyName("wind_direction_10m")]
    public int WindDirection { get; set; }

    // Порывы ветра
    [JsonPropertyName("wind_gusts_10m")]
    public double WindGusts { get; set; }
    
    [JsonPropertyName("temperature_2m")]
    public double? Temperature { get; set; }
}