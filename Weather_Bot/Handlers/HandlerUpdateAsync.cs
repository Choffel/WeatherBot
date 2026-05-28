using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Weather_Bot.Contract;
using Weather_Bot.Contract.Lublin;
using Weather_Bot.Contract.NemoPoint;

namespace Weather_Bot.Handlers;

public class HandlerUpdateAsync
{
    private readonly IMeteoService _meteoService;
    private readonly ILublinWeather _lublinWeather;
    private readonly INemoPoint _nemoPoint;

    public HandlerUpdateAsync(IMeteoService meteoService, ILublinWeather lublinWeather, INemoPoint nemoPoint)
    {
        _nemoPoint = nemoPoint;
        _lublinWeather = lublinWeather;
        _meteoService = meteoService;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        try
        {
            
            Console.WriteLine($"📩 [Handler] Получен Update! ID: {update.Id}, Тип: {update.Type}");

            var message = update.Message;
            var text = message?.Text;

            if (message == null)
            {
                Console.WriteLine("⚠️ [Handler] Update.Message пустой (возможно, это нажатие инлайн-кнопки или редактирование).");
                return;
            }

            // 🎯 ЛОГ 2: Смотрим, кто пишет и что пишет
            Console.WriteLine($"💬 [Handler] Текст: '{text}', ChatId: {message.Chat.Id}, Username: {message.From?.Username}");

            if (string.IsNullOrWhiteSpace(text))
            {
                Console.WriteLine("⚠️ [Handler] Текст сообщения пуст или состоит из пробелов. Выходим.");
                return;
            }

            if (text == "/start")
            {
                Console.WriteLine("🚀 [Handler] Сработала команда /start");
                await botClient.SendMessage(
                    chatId: message.Chat.Id,
                    text: "hello",
                    cancellationToken: cancellationToken
                );
                return;
            }

            if (text == "/wind")
            {
                Console.WriteLine("🚀 [Handler] Сработала команда /wind. Запрос к OpenMeteo...");
                var response = await _meteoService.GetWindAsync();

                if (response?.Current == null)
                {
                    Console.WriteLine("❌ [Handler] Ошибка: OpenMeteo вернул null для базовых координат.");
                    await botClient.SendMessage(
                        chatId: message.Chat.Id,
                        text: "❌ Не удалось получить данные о ветре.",
                        cancellationToken: cancellationToken
                    );
                    return;
                }

                var current = response.Current;
                var messageText =
                    $"💨 <b>Скорость ветра:</b> {current.WindSpeed} km/h\n" +
                    $"🌬 <b>Порывы:</b> {current.WindGusts} km/h\n" +
                    $"🧭 <b>Направление:</b> {current.WindDirection}°\n" +
                    $"🕒 <b>Время:</b> {current.Time}";

                await botClient.SendMessage(
                    chatId: message.Chat.Id,
                    text: messageText,
                    parseMode: ParseMode.Html, 
                    cancellationToken: cancellationToken
                );
                Console.WriteLine("✅ [Handler] Ответ на /wind успешно отправлен.");
                return;
            }

            if (text == "/Lublin")
            {
                Console.WriteLine("🚀 [Handler] Сработала команда /Lublin. Запрос погоды...");
                var response = await _lublinWeather.GetWindAndTempAsync();

                if (response?.Current == null)
                {
                    Console.WriteLine("❌ [Handler] Ошибка: OpenMeteo вернул null для Люблина.");
                    await botClient.SendMessage(
                        chatId: message.Chat.Id,
                        text: "❌ Не удалось получить данные о погоде в Люблине.",
                        cancellationToken: cancellationToken
                    );
                    return;
                } 
            
                var current = response.Current;
                var messageText =
                    $"💨 <b>Скорость ветра:</b> {current.WindSpeed} km/h\n" +
                    $"🌬 <b>Порывы:</b> {current.WindGusts} km/h\n" +
                    $"🌡️ <b>Температура:</b> {current.Temperature}°C\n" + 
                    $"🧭 <b>Направление:</b> {current.WindDirection}°\n" +
                    $"🕒 <b>Время:</b> {current.Time}";

                await botClient.SendMessage(
                    chatId: message.Chat.Id,
                    text: messageText,
                    parseMode: ParseMode.Html, 
                    cancellationToken: cancellationToken
                );
                Console.WriteLine("✅ [Handler] Ответ на /Lublin успешно отправлен.");
                return;
            }

            if (text == "/Nemo")
            {
                var response = await _nemoPoint.GetNemoPointWeatherAsync();

                if (response?.Current == null)
                {
                    await botClient.SendMessage(
                        chatId: message.Chat.Id,
                        text: "❌ Не удалось получить данные о погоде в точке Nemo.",
                        cancellationToken: cancellationToken
                    );
                    return;
                }
                
                await botClient.SendMessage(
                    chatId: message.Chat.Id,
                    text: $"Погода в точке Nemo:\n" +
                          $"💨 Скорость ветра: {response.Current.WindSpeed} km/h\n" +
                          $"🌬 Порывы: {response.Current.WindGusts} km/h\n" +
                          $"🌡️ Температура: {response.Current.Temperature}°C\n" +
                          $"🧭 Направление: {response.Current.WindDirection}°\n" +
                          $"🕒 Время: {response.Current.Time}",
                    parseMode: ParseMode.Html,
                    cancellationToken: cancellationToken
                );
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ КРИТИЧЕСКАЯ ОШИБКА ВНУТРИ HandleUpdateAsync: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }
    }
}