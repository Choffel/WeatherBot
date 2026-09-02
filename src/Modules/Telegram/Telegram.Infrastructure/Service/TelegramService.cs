using Telegram.Bot;

namespace Telegram.Infrastructure.Service;

public class TelegramService : ITelegramService
{
    private readonly ITelegramBotClient _telegramBotClient;

    public TelegramService(ITelegramBotClient telegramBotClient)
    {
        _telegramBotClient = telegramBotClient;
    }

    public Task StartASync()
    {
                
    }

    public Task GetWeatherAsync(string sity)
    {
        throw new NotImplementedException();
    }
}