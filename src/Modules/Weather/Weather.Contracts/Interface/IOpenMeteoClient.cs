namespace Weather.Contracts.Interface;

public interface IOpenMeteoClient
{
    Task<(double latitude, double longitude, string? cityName)> GetCityCoordinatesAsync(double lat, double lon, string city);
    
    Task GetWeatherAsync(double latitude, double longitude);
}