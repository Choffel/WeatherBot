using System.Net.Http.Json;
using Weather_Bot.Contract;
using Weather_Bot.DTOs;
using Weather_Bot.Enum;

namespace Weather_Bot.Service;

public class WindyService : IWeatherData
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _baseUrl = "https://api.windy.com/api/point-forecast/v2";
    private readonly IReportWeather _reportFormatter;
    
    
    public WindyService(HttpClient httpClient, IReportWeather reportFormatter)
    {
        _reportFormatter = reportFormatter;
        _httpClient = httpClient;
        _apiKey = Environment.GetEnvironmentVariable("WINDY_API_KEY") ?? string.Empty;
    }


    public async Task<WindyWeatherResponse?> GetRawForecastAsync(double lat, double lon)
    {
        var requestBody = new WindyPointRequest(
            lat, 
            lon, 
            "gfs", 
            new[] { "wind", "windGust", "pressure" }, 
            new[] { "surface" }, 
            _apiKey
        );
        
        var response = await _httpClient.PostAsJsonAsync(_baseUrl, requestBody);
        
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[Windy Error] Code: {response.StatusCode}");
            Console.WriteLine($"[Windy Error] Detail: {errorContent}");
            return null;
        }
        
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<WindyWeatherResponse>();
        }
        
        return null;
    }

    public async Task<string> GetWeatherSummaryAsync(WeatherReportType type)
    {
        string model = type == WeatherReportType.Waves ? "gfsWave" : "gfs";

        var parameters = type switch
        {
            WeatherReportType.Wind => new[] { "wind", "windGust" },
            WeatherReportType.Waves => new[] { "waves", "swell1" },
            WeatherReportType.Full => new[] { "wind", "windGust", "waves" },
            _ => new[] { "wind" }
        };
        
        var data = await GetRawForecastAsync(-58.4, -62.8);
        
        return  _reportFormatter.FormatReport(data, type);
    }
}
