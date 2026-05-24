using Telegram.Bot;

namespace Weather_Bot.Handlers;

public class HandlerErrorAsync
{
    public  async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        // Log the error (you can use any logging framework you prefer)
        Console.WriteLine($"Error occurred: {exception.Message}");
        
        // Optionally, you can send a message to the user about the error
        // await botClient.SendTextMessageAsync(chatId: <user_chat_id>, text: "An error occurred while processing your request. Please try again later.", cancellationToken: cancellationToken);
    }
}