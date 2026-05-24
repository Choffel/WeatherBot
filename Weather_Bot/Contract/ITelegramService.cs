namespace Weather_Bot.Contract;

public interface ITelegramService
{
    Task StartAsync();
    
    Task GetWindASync(double latitude, double longitude);
    
    Task SendEveningReportAsync(double latitude, double longitude);
}