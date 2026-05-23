using Telegram.Bot.Types;
using Weather_Bot.DTOs;

namespace Weather_Bot.Contract;

public interface IMessageSender
{
    
    Task<WindyWeatherResponse> GetWindAsync(double latitude, double longitude);
    
}