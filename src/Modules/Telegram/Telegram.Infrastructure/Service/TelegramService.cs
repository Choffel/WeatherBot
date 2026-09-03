using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Infrastructure.Handlers;

namespace Telegram.Infrastructure.Service;

public class TelegramService : ITelegramService
{
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly HandlerUpdateAsync _handlerUpdateAsync;
    private readonly HandlerErrorAsync _handlerErrorAsync;

    public TelegramService(ITelegramBotClient telegramBotClient
        , HandlerUpdateAsync handlerUpdateAsync
        , HandlerErrorAsync handlerErrorAsync)
    {
        _telegramBotClient = telegramBotClient;
        _handlerUpdateAsync = handlerUpdateAsync;
        _handlerErrorAsync = handlerErrorAsync;
    }

    public Task StartASync()
    {
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = new[] { UpdateType.Message, UpdateType.CallbackQuery }
        };
        
        _telegramBotClient.StartReceiving(
            receiverOptions: receiverOptions,
            updateHandler: _handlerUpdateAsync.HandlerUpdateAsync,
            errorHandle: _handlerErrorAsync.HandleErrorAsync,
            CancellationToken:
        );
        return Task.CompletedTask;
    }

    public Task GetWeatherAsync(string sity)
    {
        throw new NotImplementedException();
    }
}