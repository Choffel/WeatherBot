using System.Text.Json;
using Weather.Contracts.Interface;

namespace Weather.Infrastructure.Services;

public class OpenMeteoClient : IOpenMeteoClient
{
    private static readonly HttpClient _httpClient = new HttpClient();
    
    public  async Task<(double latitude, double longitude, string? cityName)>GetCityCoordinatesAsync (double lat, double lon,
        string city)
    {
        string safeCity = Uri.EscapeDataString(city.Trim());
        
        string url = $"https://geocoding-api.open-meteo.com/v1/search?name={safeCity}&count=1&language=ru&format=json";
        
        using var response = await _httpClient.GetAsync(url);
        
        string json = await response.Content.ReadAsStringAsync();
        
        using var doc = JsonDocument.Parse(json);

        if (!doc.RootElement.TryGetProperty("results", out var results) || results.GetArrayLength() == 0)
        {
            return (lat, lon, city);
        }
        
        var firstResult = results[0];
        double latitude = firstResult.GetProperty("latitude").GetDouble();
        double longitude = firstResult.GetProperty("longitude").GetDouble();
        string cityName = firstResult.GetProperty("name").GetString();
        
        return (latitude, longitude, cityName);
    }

    public async Task GetWeatherAsync(double lat, double lon)
    {
        string latStr = lat.ToString(System.Globalization.CultureInfo.InvariantCulture);
        string lonStr = lon.ToString(System.Globalization.CultureInfo.InvariantCulture);

        string url = $"https://api.open-meteo.com/v1/forecast?latitude={latStr}&longitude={lonStr}&current=temperature_2m,wind_speed_10m,wind_gusts_10m&wind_speed_unit=ms";

        using var response = await _httpClient.GetAsync(url);
         
        await response.Content.ReadAsStringAsync();
    }
}