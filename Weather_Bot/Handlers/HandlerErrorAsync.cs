using Telegram.Bot;

namespace Weather_Bot.Handlers;

public class HandlerErrorAsync
{
    public  Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine($"🚨 Telegram Bot Error: {exception.Message}");
        if (exception.InnerException != null)
        {
            Console.WriteLine($"🔍 Inner Exception: {exception.InnerException.Message}");
        }
        return Task.CompletedTask;
    }
}