using System.Text.Json.Serialization;

namespace Weather_Bot.DTOs;

public class WindyWeatherResponse
{
    [JsonPropertyName("ts")]
    public long[] Ts { get; set; }

    [JsonPropertyName("units")]
    public Dictionary<string, string> Units { get; set; }

    // Этот словарь соберет все остальные поля: wind_u-surface, wind_v-surface и т.д.
    [JsonExtensionData]
    public Dictionary<string, object> ExtraData { get; set; } = new();
}