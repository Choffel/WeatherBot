using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Weather_Bot.Configuration;
using Weather_Bot.Contract;
using Weather_Bot.Contract.Lublin;
using Weather_Bot.DTOs.OpenMeteoDTOs;
using Weather_Bot.Handlers;

namespace Weather_Bot.Service;

public class TelegramService : ITelegramService
{
    private readonly ITelegramBotClient _botClient;
    private readonly IMeteoService _meteoService;
    private readonly ILublinWeather _lublinWeather;

    
    //handlers
    private readonly HandlerErrorAsync _handlerErrorAsync;
    private readonly HandlerUpdateAsync _handlerUpdateAsync;

    private readonly long _chatId;
    
    private readonly BotConfiguration _config;

    public TelegramService(ITelegramBotClient botClient,
        IMeteoService meteoService, HandlerErrorAsync handlerErrorAsync, HandlerUpdateAsync handlerUpdateAsync,
        IOptions<BotConfiguration> botConfiguration, ILublinWeather lublinWeather)
    {
        _config = botConfiguration.Value;
        
        if (_config.TELEGRAM_CHAT_ID == 0)
            throw new ArgumentNullException(nameof(BotConfiguration.TELEGRAM_CHAT_ID), "TELEGRAM_CHAT_ID не задан в конфигурации.");
        
        _chatId = _config.TELEGRAM_CHAT_ID;
        
        _botClient = botClient;
        _meteoService = meteoService;
        
        _handlerErrorAsync = handlerErrorAsync;
        _handlerUpdateAsync = handlerUpdateAsync;
        
        _lublinWeather = lublinWeather;
    }


    private readonly CancellationTokenSource _cts = new(); 
    
    public Task StartAsync()
    {
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = new[] { UpdateType.Message, UpdateType.CallbackQuery }
        };

        _botClient.StartReceiving(
            updateHandler: _handlerUpdateAsync.HandleUpdateAsync,
            errorHandler: _handlerErrorAsync.HandleErrorAsync,
            receiverOptions: receiverOptions,
            cancellationToken: _cts.Token
        );

        Console.WriteLine($"✅ Long Polling запущен успешно!");
        return Task.CompletedTask;
    }
    
    
    public async Task GetWindASync()
    {
        var response = await _meteoService.GetWindAsync();

        if (response == null)
        {
            Console.WriteLine("Failed to retrieve weather data.");
            await _botClient.SendMessage(
                chatId: _chatId,
                text: "❌ Ошибка при получении данных о ветре."
            );
            return;
        }
        
        var current = response.Current;
        
        string messageText = $"💨 *Сводка погоды по координатам:* {_config.LATITUDE}, {_config.LONGITUDE}\n\n" +
                             $"🔹 *Скорость ветра:* {current.WindSpeed} km/с\n" +
                             $"🔹 *Порывы ветра:* {current.WindGusts} km/с\n" +
                             $"🔹 *Направление:* {current.WindDirection}°\n" +
                             $"🕒 *Время замера:* {current.Time}";
        
        await _botClient.SendMessage(
            chatId: _chatId,
            text: messageText,
            parseMode: ParseMode.Markdown
        );
    }

    public async Task<OpenMeteoResponse> GetLublinWeatherAsync()
    {
        var response = await _lublinWeather.GetWindAndTempAsync();

        if (response == null)
        {
            Console.WriteLine("Failed to retrieve weather data.");
        }
        
        var current = response.Current;
        
        string messageText = $"💨 *Сводка погоды по координатам:* {_config.LUBLIN_LATITUDE}, {_config.LUBLIN_LONGITUDE}\n\n" +
                            $"🔹 *Скорость ветра:* {current.WindSpeed} km/h\n" +
                            $"🔹 *Порывы ветра:* {current.WindGusts} km/h\n" +
                            $"🔹 *Направление:* {current.WindDirection}°\n" +
                            $"🌡️ *Температура:* {current.Temperature}°C\n" +
                            $"🕒 *Время замера:* {current.Time}";
        
        await _botClient.SendMessage(
            chatId: _chatId,
            text: messageText,
            parseMode: ParseMode.Markdown
        );
        
        return response;
    }

    public async Task SendEveningReportAsync()
    {
       await GetWindASync();
    }
    
}