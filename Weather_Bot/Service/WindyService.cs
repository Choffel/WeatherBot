using System.Net.Http.Json;
using Weather_Bot.Contract;
using Weather_Bot.DTOs;

namespace Weather_Bot.Service;

public class WindyService : IWeatherData
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _baseUrl = "https://api.windy.com/api/point-forecast/v2";
    
    
    public WindyService(HttpClient httpClient)
    {
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
            // Читаем текст ошибки от сервера
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

    public async Task<string> GetDrakeSummaryAsync()
    {
        var data = await GetRawForecastAsync(-58.4, -62.8);
    
        if (data == null || data.ExtraData == null) return "Error";

        
        if (data.ExtraData.TryGetValue("wind_u-surface", out var uObj) && 
            data.ExtraData.TryGetValue("wind_v-surface", out var vObj))
        {
            var uElement = (System.Text.Json.JsonElement)uObj;
            var vElement = (System.Text.Json.JsonElement)vObj;

            
            float u = uElement[0].GetSingle();
            float v = vElement[0].GetSingle();

           
            double speed = Math.Sqrt(Math.Pow(u, 2) + Math.Pow(v, 2));


            double convert = speed * 3.6;
            

            string report = $"📍 Пролив Дрейка: Текущая скорость ветра {convert:F0} км/ч";
            Console.WriteLine(report);
            return report;
        }

        return "Wind not found";
    }
}
