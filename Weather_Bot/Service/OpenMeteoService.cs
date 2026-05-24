using System.Net.Http.Json;
using Weather_Bot.Contract;
using Weather_Bot.DTOs.OpenMeteoDTOs;

namespace Weather_Bot.Service;

public class OpenMeteoService : IMeteoService
{
    private readonly HttpClient _httpClient;
    
    public OpenMeteoService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<OpenMeteoResponse?> GetWindAsync(double latitude, double longitude)
    {
        // Формируем URL с координатами и параметрами ветра в м/с (wind_speed_unit=ms)
        string url = $"https://api.open-meteo.com/v1/forecast" +
                     $"?latitude={latitude}" +
                     $"&longitude={longitude}" +
                     $"&current=wind_speed_10m,wind_direction_10m,wind_gusts_10m" +
                     $"&wind_speed_unit=ms";

        // Делаем GET запрос и сразу десериализуем в наш класс
        var response = await _httpClient.GetFromJsonAsync<OpenMeteoResponse>(url);
        return response;  
    }
}