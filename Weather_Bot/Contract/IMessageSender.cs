using Telegram.Bot.Types;
using Weather_Bot.DTOs;

namespace Weather_Bot.Contract;

public interface IMessageSender
{
    Task<WindyWeatherResponse> GetWaveAsync(double latitude, double longitude);
    
    Task<WindyWeatherResponse> GetWindAsync(double latitude, double longitude);
    
}