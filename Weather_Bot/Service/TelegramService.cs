using Microsoft.Extensions.Hosting;
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
    
    private readonly HandlerErrorAsync _handlerErrorAsync;
    private readonly HandlerUpdateAsync _handlerUpdateAsync;
    private readonly IHostApplicationLifetime _appLifetime; 

    private readonly long _chatId;
    private readonly BotConfiguration _config;

    public TelegramService(
        ITelegramBotClient botClient,
        IMeteoService meteoService, 
        HandlerErrorAsync handlerErrorAsync, 
        HandlerUpdateAsync handlerUpdateAsync,
        IOptions<BotConfiguration> botConfiguration, 
        ILublinWeather lublinWeather,
        IHostApplicationLifetime appLifetime) 
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
        _appLifetime = appLifetime;
    }

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
            cancellationToken: _appLifetime.ApplicationStopping 
        );

        Console.WriteLine($"✅ [Telegram Bot] Long Polling успешно запущен и привязан к хосту!");
        return Task.CompletedTask;
    }
    
    public async Task GetWindASync()
    {
        var response = await _meteoService.GetWindAsync();

        if (response?.Current == null)
        {
            Console.WriteLine("Failed to retrieve weather data.");
            await _botClient.SendMessage(chatId: _chatId, text: "❌ Ошибка при получении данных о ветре.");
            return;
        }
        
        var current = response.Current;
        
        string messageText = $"💨 <b>Сводка погоды по координатам:</b> {_config.LATITUDE}, {_config.LONGITUDE}\n\n" +
                             $"🔹 <b>Скорость ветра:</b> {current.WindSpeed} km/h\n" +
                             $"🔹 <b>Порывы ветра:</b> {current.WindGusts} km/h\n" +
                             $"🔹 <b>Направление:</b> {current.WindDirection}°\n" +
                             $"🕒 <b>Время замера:</b> {current.Time}";
        
        await _botClient.SendMessage(chatId: _chatId, text: messageText, parseMode: ParseMode.Html);
    }

    public async Task<OpenMeteoResponse?> GetLublinWeatherAsync()
    {
        var response = await _lublinWeather.GetWindAndTempAsync();

        if (response?.Current == null)
        {
            Console.WriteLine("Failed to retrieve Lublin weather data.");
            return response;
        }
        
        var current = response.Current;
        
        string messageText = $"💨 <b>Сводка погоды в Люблине:</b>\n\n" +
                            $"🔹 <b>Скорость ветра:</b> {current.WindSpeed} km/h\n" +
                            $"🔹 <b>Порывы ветра:</b> {current.WindGusts} km/h\n" +
                            $"🔹 <b>Направление:</b> {current.WindDirection}°\n" +
                            $"🌡️ <b>Температура:</b> {current.Temperature}°C\n" +
                            $"🕒 <b>Время замера:</b> {current.Time}";
        
        await _botClient.SendMessage(chatId: _chatId, text: messageText, parseMode: ParseMode.Html);
        
        return response;
    }

    public async Task SendEveningReportAsync()
    {
       await GetWindASync();
    }
}