using System.Net.Http.Json;
using Weather_Bot.Contract;
using Weather_Bot.DTOs;

namespace Weather_Bot.Service;

/// <summary>
/// Сервис для получения данных о погоде и волнах из API Windy
/// </summary>
public class WindyService : IWeatherData
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _baseUrl = "https://api.windy.com/api/point-forecast/v2";
    
    public WindyService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _apiKey = Environment.GetEnvironmentVariable("WINDY_API_KEY") ?? string.Empty;
        
        if (string.IsNullOrWhiteSpace(_apiKey))
            throw new InvalidOperationException("API ключ WINDY_API_KEY не задан");
    }

    /// <summary>
    /// Получает данные о ветре для указанных координат
    /// </summary>
    public async Task<WindyWeatherResponse> GetWindAsync(double latitude, double longitude)
    {
        var request = new WindyPointRequest(
            lat: latitude,
            lon: longitude,
            model: "gfs",
            parameters: new[] 
            { 
                "windGust",
                "wind"
            },
            levels: new[] { "surface" },
            key: _apiKey
        );

        Console.WriteLine(request);

        return await PostWeatherRequestAsync(request);
    }

    /// <summary>
    /// Получает данные о волнах для указанных координат /// </summary>
    public async Task<WindyWeatherResponse> GetWaveAsync(double latitude, double longitude)
    {
        try
        {
            var request = new WindyPointRequest(
                lat: latitude,
                lon: longitude,
                model: "gfs",
                parameters: new[] 
                { 
                    "waves"
                },
                levels: new[] { "surface" },
                key: _apiKey
            );
            

            
            return await PostWeatherRequestAsync(request);

        }
        catch (Exception response)
        {
            Console.WriteLine($"❌ Ошибка при формировании запроса: {response.Message}");
            throw;
        }
    }

    /// <summary>
    /// Отправляет запрос к API Windy и получает ответ
    /// </summary>
    private async Task<WindyWeatherResponse> PostWeatherRequestAsync(WindyPointRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(_baseUrl, request);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<WindyWeatherResponse>() 
                    ?? new WindyWeatherResponse();
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"Ошибка API Windy: {response.StatusCode}. {errorContent}");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"❌ Ошибка при запросе к API: {ex.Message}");
            throw;
        }
    }
}
