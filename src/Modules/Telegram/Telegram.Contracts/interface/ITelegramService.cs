namespace Telegram.Contracts.interface;

public interface ITelegramService
{
    Task StartASync();
    
    Task GetWeatherAsync(string sity);
    
}