using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Weather_Bot.Contract;
using Weather_Bot.Enum;

namespace Weather_Bot.Service;

public class NotificationService : IMessageSender
{
    private readonly ITelegramBotClient _botClient;
    private readonly IWeatherData _weatherData;
    
    public NotificationService(string token,  IWeatherData weatherData)
    {
        _weatherData = weatherData;
        _botClient = new TelegramBotClient(token);
    }
    
    public async Task SendAsync()
    {
        var chatIdEnv = Environment.GetEnvironmentVariable("TELEGRAM_DEFAULT_CHAT_ID");
        
        await _botClient.SendMessage(chatId: chatIdEnv, text: "Hello World");
    }

    public async Task StartAsync()
    {
        using var cts = new CancellationTokenSource();

        
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = Array.Empty<UpdateType>() 
        };

        _botClient.StartReceiving(
            
            updateHandler: (client, update, ct) => HandleUpdateAsync(client, update, ct), 
            
            errorHandler: (client, ex, ct) => HandlePollingErrorAsync(client, ex, ct),
            receiverOptions: receiverOptions,
            cancellationToken: cts.Token
        );

        var me = await _botClient.GetMe();
        
        Console.WriteLine($"Бот @{me.Username} запущен и слушает...");
    
        
        await Task.Delay(-1);
    }
    
    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Telegram.Bot.Types.Update update, CancellationToken cancellationToken)
    {
        
        if (update.Message is not { Text: { } messageText } message) return;

        var chatId = message.Chat.Id;
        
        switch (messageText.ToLower())
        {
            case "/start":
                await botClient.SendMessage(
                    chatId: chatId,
                    text: "Привет! Я погодный бот. Выберите регион:",
                    replyMarkup: ReplyMarkups.GetDrakeKeyboard(),
                    cancellationToken: cancellationToken);
                break;
            
            case "/drake":
                var report = await _weatherData.GetWeatherSummaryAsync(WeatherReportType.Full);
                await botClient.SendMessage(chatId, report, cancellationToken: cancellationToken);
                break;
        }
    }
    
    private async Task HandleCallbackQueryAsync(ITelegramBotClient botClient, Telegram.Bot.Types.CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        var chatId = callbackQuery.Message?.Chat.Id;
        if (chatId == null) return;

        string responseText = callbackQuery.Data switch
        {
            "report_wind" => await _weatherData.GetWeatherSummaryAsync(WeatherReportType.Wind),
            "report_waves" => await _weatherData.GetWeatherSummaryAsync(WeatherReportType.Waves),
            "report_full" => await _weatherData.GetWeatherSummaryAsync(WeatherReportType.Full),
            _ => "Неизвестная команда"
        };

        await botClient.SendMessage(chatId.Value, responseText, cancellationToken: cancellationToken);
    }
    
    private Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine("Ошибка API: " + exception.Message);
        return Task.CompletedTask;
    }

    public  Task StopAsync()
    {
        return  Task.CompletedTask;
    }
}