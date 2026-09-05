using Telegram.Bot;
using Telegram.Bot.Types;

namespace Telegram.Infrastructure.Handlers;

public class HandlerUpdateAsync
{
    private readonly ITelegramBotClient _botClient;
    
    public HandlerUpdateAsync(ITelegramBotClient botClient)
    {
        _botClient = botClient;
    }
    
    public async Task HandleUpdateAsync(ITelegramBotClient botClient,Update update, CancellationToken ct)
    {
        var message = update.Message;
        var text = message.Text;

        if (text == null)
        {
            await _botClient.SendMessage(message.Chat.Id, "Please send a text message.", cancellationToken: ct);
        }
        
        if(text == "/start")
        {
            await _botClient.SendMessage(message.Chat.Id, " write the city", cancellationToken: ct);
        }
    }
}