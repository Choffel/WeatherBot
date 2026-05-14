using System.Text.Json.Serialization;

namespace Weather_Bot.DTOs;

public record WindyPointRequest(
    [property: JsonPropertyName("lat")] double lat,
    [property: JsonPropertyName("lon")] double lon,
    [property: JsonPropertyName("model")] string model,
    [property: JsonPropertyName("parameters")] string[] parameters,
    [property: JsonPropertyName("levels")] string[] levels,
    [property: JsonPropertyName("key")] string key
    );