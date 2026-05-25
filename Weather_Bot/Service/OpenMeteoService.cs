using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using Weather_Bot.Configuration;
using Weather_Bot.Contract;
using Weather_Bot.Contract.Lublin;
using Weather_Bot.DTOs.OpenMeteoDTOs;

namespace Weather_Bot.Service;

public class OpenMeteoService : IMeteoService, ILublinWeather
{
    private readonly HttpClient _httpClient;
    private readonly BotConfiguration _config;
    
    public OpenMeteoService(HttpClient httpClient, IOptions<BotConfiguration> botConfiguration)
    {
        _config = botConfiguration.Value;
        _httpClient = httpClient;
    }
    
    public async Task<OpenMeteoResponse?> GetWindAsync()
    {
        
        string url = $"https://api.open-meteo.com/v1/forecast" +
                     $"?latitude={_config.LATITUDE}" +
                     $"&longitude={_config.LONGITUDE}" +
                     $"&current=wind_speed_10m,wind_direction_10m,wind_gusts_10m" +
                     $"&wind_speed_unit=kmh";

        
        
        var response = await _httpClient.GetFromJsonAsync<OpenMeteoResponse>(url);
        return response;  
    }
    
    public async  Task<OpenMeteoResponse?> GetWindAndTempAsync()
    {
        string url =  $"https://api.open-meteo.com/v1/forecast" +
                      $"?latitude={_config.LUBLIN_LATITUDE}" +  // Сначала Широта (Latitude)
                      $"&longitude={_config.LUBLIN_LONGITUDE}" + // Затем Долгота (Longitude)
                      $"&current=temperature_2m,wind_speed_10m,wind_direction_10m,wind_gusts_10m" +
                      $"&wind_speed_unit=kmh";
        
        var response = await _httpClient.GetFromJsonAsync<OpenMeteoResponse>(url);
        return response;
    }
}