using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Weather_Bot.Contract;
using Weather_Bot.DTOs.OpenMeteoDTOs;
using Weather_Bot.Handlers;

namespace Weather_Bot.Service;

public class TelegramService : ITelegramService
{
    private readonly ITelegramBotClient _botClient;
    private readonly IMeteoService _meteoService;

    
    //handlers
    private readonly HandlerErrorAsync _handlerErrorAsync;
    private readonly HandlerUpdateAsync _handlerUpdateAsync;

    private readonly long _chatId;
    
    
    //params 
    private double Latitude => Environment.GetEnvironmentVariable("LATITUDE") != null
        ? double.Parse(Environment.GetEnvironmentVariable("LATITUDE")!)
        : throw new ArgumentNullException("LATITUDE environment variable is not set.");
    
    private double Longitude => Environment.GetEnvironmentVariable("LONGITUDE") != null
        ? double.Parse(Environment.GetEnvironmentVariable("LONGITUDE")!)
        : throw new ArgumentNullException("LONGITUDE environment variable is not set.");
    
    

    public TelegramService(ITelegramBotClient botClient,
        IMeteoService meteoService, HandlerErrorAsync handlerErrorAsync, HandlerUpdateAsync handlerUpdateAsync)
    {
        _chatId = Environment.GetEnvironmentVariable("TELEGRAM_CHAT_ID") != null
            ? long.Parse(Environment.GetEnvironmentVariable("TELEGRAM_CHAT_ID")!)
            : throw new ArgumentNullException("TELEGRAM_CHAT_ID environment variable is not set.");
        
        _botClient = botClient;
        _meteoService = meteoService;
        
        _handlerErrorAsync = handlerErrorAsync;
        _handlerUpdateAsync = handlerUpdateAsync;
    }


    // add parametrs 
    public async Task StartAsync()
    {
         using var cts = new CancellationTokenSource();
         
         var receiverOptions = new ReceiverOptions
         {
             AllowedUpdates = new[] { UpdateType.Message, UpdateType.CallbackQuery }
         };

         _botClient.StartReceiving(
             updateHandler: _handlerUpdateAsync.HandleUpdateAsync,
             errorHandler: _handlerErrorAsync.HandleErrorAsync ,
             receiverOptions: receiverOptions,
             cancellationToken: cts.Token
         );

         var botInfo = await _botClient.GetMe(cancellationToken: cts.Token);
         
         Console.WriteLine($"✅ Бот @{botInfo.Username} запущен. Нажмите Ctrl+C для выхода.");

         await Task.Delay(Timeout.Infinite, cts.Token);
    }
    
    
    public async Task GetWindASync(double latitude, double longitude)
    {
        var response = await _meteoService.GetWindAsync(latitude, longitude);

        if (response == null)
        {
            Console.WriteLine("Failed to retrieve weather data.");
            return;
        }
        
        
        string messageText = $"💨 *Сводка погоды по координатам:* {latitude}, {longitude}\n\n" +
                             $"🔹 *Скорость ветра:* {CurrentWeatherData.WindSpeed} м/с\n" +
                             $"🔹 *Порывы ветра:* {CurrentWeatherData.WindGusts} м/с\n" +
                             $"🔹 *Направление:* {CurrentWeatherData.WindDirection}°\n" +
                             $"🕒 *Время замера:* {CurrentWeatherData.Time}";
        
        _botClient.SendMessage(
            chatId: _chatId,
            text: messageText,
            parseMode: ParseMode.Markdown
        );
    }

    public async  Task SendEveningReportAsync(double latitube, double longitube)
    {
       await GetWindASync(latitube, longitube);
    }
}