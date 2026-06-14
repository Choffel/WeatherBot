using System.Text.Json.Serialization;

namespace Weather_Bot.DTOs.IssDTO;

public class SatelliteStateResponse
{
    [JsonPropertyName("latitude")]
    public double Latitude { get; set; }
    
    [JsonPropertyName("longitude")]
    public double Longitude { get; set; }
}