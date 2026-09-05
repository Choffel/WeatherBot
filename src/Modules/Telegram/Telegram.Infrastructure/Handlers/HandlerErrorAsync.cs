using Telegram.Bot;

namespace Telegram.Infrastructure.Handlers;

//TODO: To change
public class HandlerErrorAsync
{
    public  Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        if (exception.InnerException != null)
        {
            Console.WriteLine($"Inner Exception: {exception.InnerException.Message}");
        }
        return Task.CompletedTask;
    }
}