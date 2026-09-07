using System.Collections.Concurrent;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Weather.Contracts.Interface;

namespace Telegram.Infrastructure.Handlers;

public class HandlerUpdateAsync
{
    private readonly ITelegramBotClient _bot;
    private readonly IOpenMeteoClient _openMeteoClient;

    
    private static readonly ConcurrentDictionary<long, bool> WaitingCity = new();

    public HandlerUpdateAsync(ITelegramBotClient bot, IOpenMeteoClient openMeteoClient)
    {
        _bot = bot;
        _openMeteoClient = openMeteoClient;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
    {
        if (update.Type != UpdateType.Message || update.Message?.Text is null)
            return;

        var chatId = update.Message.Chat.Id;
        var text = update.Message.Text.Trim();

        if (text.Equals("/start", StringComparison.OrdinalIgnoreCase))
        {
            WaitingCity[chatId] = true;
            await _bot.SendMessage(chatId, "Введите город", cancellationToken: ct);
            return;
        }

        if (!WaitingCity.ContainsKey(chatId))
            return;
        
        var (lat, lon, cityName) = await _openMeteoClient.GetCityCoordinatesAsync(0, 0, text);
        
        //must be awaited Task<string> in interface and implementation
        var weatherSummary =  _openMeteoClient.GetWeatherAsync(lat, lon);

        WaitingCity.TryRemove(chatId, out _);

        await _bot.SendMessage(
            chatId,
            $"Погода для {cityName ?? text}:\n{weatherSummary}",
            cancellationToken: ct);
    }
}