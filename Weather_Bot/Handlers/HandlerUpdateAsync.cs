using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Weather_Bot.Contract;
using Weather_Bot.Contract.Lublin;

namespace Weather_Bot.Handlers;

public class HandlerUpdateAsync
{
    private readonly IMeteoService _meteoService;
    private readonly ILublinWeather _lublinWeather;

    public HandlerUpdateAsync(IMeteoService meteoService, ILublinWeather lublinWeather)
    {
        _lublinWeather = lublinWeather;
        _meteoService = meteoService;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        try
        {
            var message = update.Message;
            var text = message?.Text;

            if (message?.Chat == null || string.IsNullOrWhiteSpace(text))
                return;

            if (text == "/start")
            {
                await botClient.SendMessage(
                    chatId: message.Chat.Id,
                    text: "Бот запущен. Используй /wind для отчета по ветру.\n" +
                          "Используй /Lublin для отчета по ветру и температуре в Люблине.",
                    cancellationToken: cancellationToken
                );
                return;
            }

            if (text == "/wind")
            {
                var response = await _meteoService.GetWindAsync();

                if (response?.Current == null)
                {
                    await botClient.SendMessage(
                        chatId: message.Chat.Id,
                        text: "❌ Не удалось получить данные о ветре.",
                        cancellationToken: cancellationToken
                    );
                    return;
                }

                var current = response.Current;
                
                // Перевели на HTML разметку (<b> вместо *)
                var messageText =
                    $"💨 <b>Скорость ветра:</b> {current.WindSpeed} km/h\n" +
                    $"🌬 <b>Порывы:</b> {current.WindGusts} km/h\n" +
                    $"🧭 <b>Направление:</b> {current.WindDirection}°\n" +
                    $"🕒 <b>Время:</b> {current.Time}";

                await botClient.SendMessage(
                    chatId: message.Chat.Id,
                    text: messageText,
                    parseMode: ParseMode.Html, // 👈 ИСПРАВЛЕНО НА HTML
                    cancellationToken: cancellationToken
                );
                return;
            }

            if (text == "/Lublin")
            {
                var response = await _lublinWeather.GetWindAndTempAsync();

                if (response?.Current == null)
                {
                    await botClient.SendMessage(
                        chatId: message.Chat.Id,
                        text: "❌ Не удалось получить данные о погоде в Люблине.",
                        cancellationToken: cancellationToken
                    );
                    return;
                } 
            
                var current = response.Current;
            
                // Перевели на HTML разметку (<b> вместо *)
                var messageText =
                    $"💨 <b>Скорость ветра:</b> {current.WindSpeed} km/h\n" +
                    $"🌬 <b>Порывы:</b> {current.WindGusts} km/h\n" +
                    $"🌡️ <b>Температура:</b> {current.Temperature}°C\n" + 
                    $"🧭 <b>Направление:</b> {current.WindDirection}°\n" +
                    $"🕒 <b>Время:</b> {current.Time}";

                await botClient.SendMessage(
                    chatId: message.Chat.Id,
                    text: messageText,
                    parseMode: ParseMode.Html, // 👈 ИСПРАВЛЕНО НА HTML
                    cancellationToken: cancellationToken
                );
                return;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Ошибка внутри HandleUpdateAsync: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }
    }
}