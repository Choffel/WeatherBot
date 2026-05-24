using Telegram.Bot;
using Telegram.Bot.Types;
using Weather_Bot.Contract;

namespace Weather_Bot.Handlers;

public class HandlerUpdateAsync
{
    private readonly ITelegramService _telegramService;

    public HandlerUpdateAsync(ITelegramService telegramService)
    {
        _telegramService = telegramService;
    }
    
       public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
       {
           if (update.Message?.Text == "/start")
           {
               await _telegramService.StartAsync();
           }
           else if (update.Message?.Text == "/wind")
           {
               double latitude = double.Parse(Environment.GetEnvironmentVariable("LATITUDE") ?? "-58.4");
               double longitude = double.Parse(Environment.GetEnvironmentVariable("LONGITUDE") ?? "-62.8");
                
               await _telegramService.GetWindASync(latitude, longitude);
           }
       }
}