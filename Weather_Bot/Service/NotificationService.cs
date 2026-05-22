using System.Text.Json;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Weather_Bot.Contract;
using Weather_Bot.DTOs;

namespace Weather_Bot.Service;

/// <summary>
/// Сервис для отправки уведомлений через Telegram Bot
/// </summary>
public class NotificationService : IMessageSender
{
    private readonly ITelegramBotClient _botClient;
    private readonly IWeatherData _weatherData;
    private double _latitude;
    private double _longitude;

    public NotificationService(ITelegramBotClient botClient, IWeatherData weatherData)
    {
        _botClient = botClient ?? throw new ArgumentNullException(nameof(botClient));
        _weatherData = weatherData ?? throw new ArgumentNullException(nameof(weatherData));
        
        // Загружаем координаты из переменных окружения
        _latitude = double.Parse(Environment.GetEnvironmentVariable("LATITUDE") ?? "-58.4");
        _longitude = double.Parse(Environment.GetEnvironmentVariable("LONGITUDE") ?? "-62.8");
    }

    
    public async Task<WindyWeatherResponse> GetWindAsync(double latitude, double longitude)
    {
        return await _weatherData.GetWindAsync(latitude, longitude);
    }

   
    public async Task<WindyWeatherResponse> GetWaveAsync(double latitude, double longitude)
    {
        return await _weatherData.GetWaveAsync(latitude, longitude);
    }

    
    public async Task StartAsync()
    {
        using var cts = new CancellationTokenSource();
        
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = new[] { UpdateType.Message, UpdateType.CallbackQuery }
        };

        _botClient.StartReceiving(
            updateHandler: HandleUpdateAsync,
            errorHandler: HandleErrorAsync,
            receiverOptions: receiverOptions,
            cancellationToken: cts.Token
        );

        var botInfo = await _botClient.GetMe(cancellationToken: cts.Token);
        
        Console.WriteLine($"✅ Бот @{botInfo.Username} запущен. Нажмите Ctrl+C для выхода.");

        // Держим бота запущенным
        await Task.Delay(Timeout.Infinite, cts.Token);
    }

    
    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        try
        {
            if (update.Message?.Text == "/start")
            {
                await SendWeatherReportAsync(update.Message.Chat.Id, cancellationToken);
            }
            else if (update.Message?.Text == "/wind")
            {
                await SendWindReportAsync(update.Message.Chat.Id, cancellationToken);
            }
            else if (update.Message?.Text == "/waves")
            {
                await SendWaveReportAsync(update.Message.Chat.Id, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Ошибка при обработке обновления: {ex.Message}");
        }
    }

   
    private async Task SendWeatherReportAsync(long chatId, CancellationToken cancellationToken)
    {
        try
        {
            var windData = await GetWindAsync(_latitude, _longitude);
            var waveData = await GetWaveAsync(_latitude, _longitude);

            var message = FormatWeatherReport(windData, waveData);
            
            await _botClient.SendMessage(
                chatId: chatId,
                text: message,
                parseMode: ParseMode.Html,
                cancellationToken: cancellationToken
            );
        }
        catch (Exception ex)
        {
            await _botClient.SendMessage(
                chatId: chatId,
                text: $"❌ Ошибка при получении данных: {ex.Message}",
                cancellationToken: cancellationToken
            );
        }
    }

   
    private async Task SendWindReportAsync(long chatId, CancellationToken cancellationToken)
    {
        try
        {
            var windData = await GetWindAsync(_latitude, _longitude);
            var message = FormatWindReport(windData);
            
            await _botClient.SendMessage(
                chatId: chatId,
                text: message,
                parseMode: ParseMode.Html,
                cancellationToken: cancellationToken
            );
        }
        catch (Exception ex)
        {
            await _botClient.SendMessage(
                chatId: chatId,
                text: $"❌ Ошибка при получении данных о ветре: {ex.Message}",
                cancellationToken: cancellationToken
            );
        }
    }

    private async Task SendWaveReportAsync(long chatId, CancellationToken cancellationToken)
    {
        try
        {
            var waveData = await GetWaveAsync(_latitude, _longitude);
            var message = FormatWaveReport(waveData);
            
            await _botClient.SendMessage(
                chatId: chatId,
                text: message,
                parseMode: ParseMode.Html,
                cancellationToken: cancellationToken
            );
        }
        catch (Exception ex)
        {
            await _botClient.SendMessage(
                chatId: chatId,
                text: $"❌ Ошибка при получении данных о волнах: {ex.Message}",
                cancellationToken: cancellationToken
            );
        }
    }

    /// <summary>
    /// Обрабатывает ошибки при получении обновлений
    /// </summary>
    private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine($"❌ Ошибка Telegram Bot: {exception.Message}");
        return Task.CompletedTask;
    }

    /// <summary>
    /// Форматирует полный отчет о погоде
    /// </summary>
    private string FormatWeatherReport(WindyWeatherResponse windData, WindyWeatherResponse waveData)
    {
        return $"""
            <b>🌍 Отчет о погоде</b>
            📍 Координаты: {_latitude}, {_longitude}
            
            <b>💨 Ветер:</b>
            {FormatWindData(windData)}
            
            <b>🌊 Волны:</b>
            {FormatWaveData(waveData)}
            """;
    }

    /// <summary>
    /// Форматирует отчет о ветре
    /// </summary>
    private string FormatWindReport(WindyWeatherResponse data)
    {
        return $"""
            <b>💨 Отчет о ветре</b>
            📍 Координаты: {_latitude}, {_longitude}
            
            {FormatWindData(data)}
            """;
    }

    /// <summary>
    /// Форматирует отчет о волнах
    /// </summary>
    private string FormatWaveReport(WindyWeatherResponse data)
    {
        return $"""
            <b>🌊 Отчет о волнах</b>
            📍 Координаты: {_latitude}, {_longitude}
            
            {FormatWaveData(data)}
            """;
    }

    /// <summary>
    /// Форматирует данные о ветре из ответа API
    /// </summary>
private string FormatWindData(WindyWeatherResponse data)
    {
        var details = new List<string>();
    
        if (data?.ExtraData != null &&
            data.ExtraData.TryGetValue("wind_u-surface", out var windUObj) &&
            data.ExtraData.TryGetValue("wind_v-surface", out var windVObj) &&
            windUObj is JsonElement windUElement &&
            windVObj is JsonElement windVElement &&
            windUElement.TryGetDouble(out var windU) &&
            windVElement.TryGetDouble(out var windV))
        {
            // Рассчитываем скорость ветра в м/с
            var windSpeedMs = Math.Sqrt(Math.Pow(windU, 2) + Math.Pow(windV, 2));
            
            // Конвертируем в км/ч
            var windSpeedKmh = windSpeedMs * 3.6;
    
            details.Add($"💨 Скорость: {windSpeedKmh:F1} км/ч ({windSpeedMs:F1} м/с)");
    
            // Добавляем информацию о порывах, если она есть
            if (data.ExtraData.TryGetValue("windGust-surface", out var windGustObj) &&
                windGustObj is JsonElement windGustElement &&
                windGustElement.TryGetDouble(out var windGustMs))
            {
                var windGustKmh = windGustMs * 3.6;
                details.Add($"🌪️ Порывы до: {windGustKmh:F1} км/ч ({windGustMs:F1} м/с)");
            }
        }
    
        return details.Count > 0 ? string.Join("\n", details) : "Данные о ветре недоступны";
    }
    

    /// <summary>
    /// Форматирует данные о волнах из ответа API
    /// </summary>
    private string FormatWaveData(WindyWeatherResponse data)
    {
        var details = new List<string>();
        
        if (data?.ExtraData != null)
        {
            if (data.ExtraData.TryGetValue("waves-surface", out var waves))
                details.Add($"Высота волн: {waves}");
            
            if (data.ExtraData.TryGetValue("waveHeight-surface", out var waveHeight))
                details.Add($"Высота волн (доп.): {waveHeight}");
        }

        return details.Count > 0 ? string.Join("\n", details) : "Данные недоступны";
    }
    
}