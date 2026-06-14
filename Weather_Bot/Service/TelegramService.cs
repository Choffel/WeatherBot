using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Weather_Bot.Configuration;
using Weather_Bot.Contract;
using Weather_Bot.Contract.Iss;
using Weather_Bot.Contract.Lublin;
using Weather_Bot.DTOs.IssDTO;
using Weather_Bot.DTOs.OpenMeteoDTOs;
using Weather_Bot.Handlers;
using Weather_Bot.Formatters;

namespace Weather_Bot.Service;

public class TelegramService : ITelegramService
{
    private readonly ITelegramBotClient _botClient;
    private readonly IMeteoService _meteoService;
    private readonly ILublinWeather _lublinWeather;
    private readonly ISatelliteStateService _satelliteStateService;
    
    private readonly HandlerErrorAsync _handlerErrorAsync;
    private readonly HandlerUpdateAsync _handlerUpdateAsync;
    private readonly IHostApplicationLifetime _appLifetime;
    private readonly WeatherMessageFormatter _messageFormatter;

    private readonly long _chatId;
    private readonly BotConfiguration _config;

    public TelegramService(
        ISatelliteStateService satelliteStateService,
        ITelegramBotClient botClient,
        IMeteoService meteoService, 
        HandlerErrorAsync handlerErrorAsync, 
        HandlerUpdateAsync handlerUpdateAsync,
        IOptions<BotConfiguration> botConfiguration, 
        ILublinWeather lublinWeather,
        IHostApplicationLifetime appLifetime,
        WeatherMessageFormatter messageFormatter) 
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
        _satelliteStateService = satelliteStateService;
        _appLifetime = appLifetime;
        _messageFormatter = messageFormatter;
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
        
        var messageText = _messageFormatter.FormatWeatherWithCoordinatesMessage(
            response.Current,
            _config.LATITUDE,
            _config.LONGITUDE
        );
        
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
        
        var messageText = _messageFormatter.FormatWeatherWithCoordinatesMessage(
            response.Current,
            _config.LUBLIN_LATITUDE,
            _config.LUBLIN_LONGITUDE,
            "Люблине"
        );
        
        await _botClient.SendMessage(chatId: _chatId, text: messageText, parseMode: ParseMode.Html);
        
        return response;
    }

    public async Task GetWeatherUnderIss(long chatId, CancellationToken cancellationToken)
    {
        Console.WriteLine("🚀 [TelegramService] Запрашиваем координаты МКС из Redis...");
        
        var satelliteState = await _satelliteStateService.GetSatelliteStateAsync(cancellationToken);
        
        if (satelliteState == null)
        {
            Console.WriteLine("❌ [TelegramService] Не удалось получить данные из Redis.");
            await _botClient.SendMessage(
                chatId: chatId, 
                text: "❌ Данные о местоположении МКС сейчас недоступны.", 
                cancellationToken: cancellationToken
            );
            return;
        }

        double latitude = satelliteState.Latitude;
        double longitude = satelliteState.Longitude;
        
        var response = await _meteoService.GetWeatherIssAsync(latitude, longitude);

        if (response?.Current == null)
        {
            Console.WriteLine("❌ [TelegramService] OpenMeteo вернул null для координат МКС.");
            await _botClient.SendMessage(
                chatId: chatId, 
                text: "❌ Не удалось получить данные о погоде под МКС.", 
                cancellationToken: cancellationToken
            );
            return;
        }
        
        var messageText = _messageFormatter.FormatIssWeatherMessage(response.Current, latitude, longitude);

        await _botClient.SendMessage(
            chatId: chatId,
            text: messageText,
            parseMode: ParseMode.Html,
            cancellationToken: cancellationToken
        );

        Console.WriteLine("✅ [TelegramService] Ответ на /Iss успешно отправлен.");
    }


    public async Task SendEveningReportAsync()
    {
       await GetWindASync();
    }
}