using Telegram.Bot.Types;

namespace Weather_Bot.Contract;

public interface IMessageSender
{
    Task SendAsync();
    
    Task StartAsync();
    
    Task StopAsync();
}