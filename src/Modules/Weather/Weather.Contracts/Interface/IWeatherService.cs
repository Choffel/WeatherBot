namespace Weather.Contracts.Interface;

public interface IWeatherService
{
    Task<WeatherResponse> GetWeatherAsync(string location);
}