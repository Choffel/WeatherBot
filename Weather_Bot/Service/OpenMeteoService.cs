using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using Weather_Bot.Configuration;
using Weather_Bot.Contract;
using Weather_Bot.Contract.Lublin;
using Weather_Bot.DTOs.OpenMeteoDTOs;

namespace Weather_Bot.Service;

public class OpenMeteoService : IMeteoService, ILublinWeather
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly BotConfiguration _config;
    
    
    public OpenMeteoService(IHttpClientFactory httpClientFactory, IOptions<BotConfiguration> botConfiguration)
    {
        _config = botConfiguration.Value;
        _httpClientFactory = httpClientFactory;
    }
    
    public async Task<OpenMeteoResponse?> GetWindAsync()
    {
        string url = $"https://api.open-meteo.com/v1/forecast" +
                     $"?latitude={_config.LATITUDE}" +
                     $"&longitude={_config.LONGITUDE}" +
                     $"&current=wind_speed_10m,wind_direction_10m,wind_gusts_10m" +
                     $"&wind_speed_unit=kmh";

        
        var httpClient = _httpClientFactory.CreateClient();
        
        var response = await httpClient.GetFromJsonAsync<OpenMeteoResponse>(url);
        return response;  
    }

    public Task<OpenMeteoResponse?> GetWeatherIssAsync(double latitude, double longitude)
    {
        string url = $"https://api.open-meteo.com/v1/forecast" +
                     $"?latitude={latitude}" +
                     $"&longitude={longitude}" +
                     $"&current=temperature_2m,wind_speed_10m,wind_direction_10m,wind_gusts_10m" +
                     $"&wind_speed_unit=kmh";

        
        var httpClient = _httpClientFactory.CreateClient();
        
        return httpClient.GetFromJsonAsync<OpenMeteoResponse>(url);
    }

    public async Task<OpenMeteoResponse?> GetWindAndTempAsync()
    {
        string url = $"https://api.open-meteo.com/v1/forecast" +
                     $"?latitude={_config.LUBLIN_LATITUDE}" +  
                     $"&longitude={_config.LUBLIN_LONGITUDE}" + 
                     $"&current=temperature_2m,wind_speed_10m,wind_direction_10m,wind_gusts_10m" +
                     $"&wind_speed_unit=kmh";
        
        
        var httpClient = _httpClientFactory.CreateClient();
        
        var response = await httpClient.GetFromJsonAsync<OpenMeteoResponse>(url);
        return response;
    }
}