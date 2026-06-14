using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Weather_Bot.Contract;
using Weather_Bot.Contract.Iss;
using Weather_Bot.Contract.Lublin;
using Weather_Bot.Formatters;

namespace Weather_Bot.Handlers;

public class HandlerUpdateAsync
{
    private readonly IMeteoService _meteoService;
    private readonly ILublinWeather _lublinWeather;
    private readonly ISatelliteStateService _satelliteStateService;
    private readonly WeatherMessageFormatter _messageFormatter;

    public HandlerUpdateAsync(
        IMeteoService meteoService,
        ILublinWeather lublinWeather,
        ISatelliteStateService satelliteStateService,
        WeatherMessageFormatter messageFormatter)
    {
        _satelliteStateService = satelliteStateService;
        _lublinWeather = lublinWeather;
        _meteoService = meteoService;
        _messageFormatter = messageFormatter;
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

                var messageText = _messageFormatter.FormatWindMessage(response.Current);

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
            
                var messageText = _messageFormatter.FormatLublinWeatherMessage(response.Current);

                await botClient.SendMessage(
                    chatId: message.Chat.Id,
                    text: messageText,
                    parseMode: ParseMode.Html, 
                    cancellationToken: cancellationToken
                );
                Console.WriteLine("✅ [Handler] Ответ на /Lublin успешно отправлен.");
                return;
            }


            if (text == "/Iss")
            {
                Console.WriteLine("🚀 [Handler] Сработала команда /Iss. Запрашиваем координаты из Redis...");

                
                var satellitePosition = await _satelliteStateService.GetSatelliteStateAsync(cancellationToken);

                if (satellitePosition == null)
                {
                    Console.WriteLine("❌ [Handler] Ошибка: Не удалось получить координаты МКС из Redis.");
                    await botClient.SendMessage(
                        chatId: message.Chat.Id,
                        text: "❌ Данные о местоположении МКС сейчас недоступны.",
                        cancellationToken: cancellationToken
                    );
                    return;
                }
                Console.WriteLine("✅ [Handler] Ответ на /Iss успешно отправлен.");
                return;
            }

            // ЛОГ 3: Если пришла команда, которую бот не знает (например, просто "привет")
            Console.WriteLine($"❓ [Handler] Неизвестная команда: {text}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ КРИТИЧЕСКАЯ ОШИБКА ВНУТРИ HandleUpdateAsync: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }
    }
}