using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Weather_Bot.Contract;
using Weather_Bot.Service;

namespace Weather_Bot;

/// <summary>
/// Расширение для регистрации сервисов приложения
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Добавляет сервисы Weather Bot
    /// </summary>
    public static IServiceCollection AddWeatherBotServices(this IServiceCollection services)
    {
        // Регистрация форматирования отчётов
        services.AddSingleton<IReportWeather, Report>();
        
        
        // Регистрация HTTP клиента для Windy API
        services.AddHttpClient<WindyService>();
        
        // Регистрация Telegram Bot клиента (ДОЛЖНО БЫТЬ ДО NotificationService!)
        services.AddSingleton<ITelegramBotClient>(sp =>
        {
            var token = GetRequiredEnv("TELEGRAM_BOT_TOKEN");
            return new TelegramBotClient(token);
        });
        
        // Регистрация сервисов погоды
        services.AddSingleton<IWeatherData>(sp => sp.GetRequiredService<WindyService>());
        
        // Регистрация сервиса уведомлений (требует ITelegramBotClient и IWeatherData)
        services.AddSingleton<IMessageSender, NotificationService>();
        
        return services;
    }

    /// <summary>
    /// Получает значение переменной окружения или выбрасывает исключение
    /// </summary>
    private static string GetRequiredEnv(string name)
    {
        var value = Environment.GetEnvironmentVariable(name);

        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidOperationException(
                $"❌ Ошибка конфигурации: переменная окружения '{name}' не задана. " +
                $"Проверьте файл .env в корне проекта.");

        return value;
    }
}
