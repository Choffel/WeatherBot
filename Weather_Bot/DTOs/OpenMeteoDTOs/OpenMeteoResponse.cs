using System.Text.Json.Serialization;

namespace Weather_Bot.DTOs.OpenMeteoDTOs;

public class OpenMeteoResponse
{
    [JsonPropertyName("latitude")]
    public double Latitude { get; set; }

    [JsonPropertyName("longitude")]
    public double Longitude { get; set; }

    [JsonPropertyName("current")]
    public CurrentWeatherData Current { get; set; }
}