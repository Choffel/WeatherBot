using Weather_Bot.DTOs.IssDTO;
using Weather_Bot.DTOs.OpenMeteoDTOs;

namespace Weather_Bot.Contract;

public interface ITelegramService
{
    Task StartAsync();
    
    Task GetWindASync();
    
    Task SendEveningReportAsync();

    Task<OpenMeteoResponse> GetLublinWeatherAsync();
     
    Task GetWeatherUnderIss(long chatId, CancellationToken cancellationToken);
}