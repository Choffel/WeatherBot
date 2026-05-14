using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Weather_Bot.Contract;

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
                await botClient.SendMessage(chatId, "Напиши /drake, чтобы узнать обстановку в проливе.");
                break;
            case "/drake":
                var drakeSummary = await _weatherData.GetWeatherSummaryAsync(Enum.WeatherReportType.Full);
                await botClient.SendMessage(chatId, drakeSummary);
                break;
            default:
                await botClient.SendMessage(chatId, "я не понимаю эту команду. Попробуй /start или /drake.");
                break;
        }
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