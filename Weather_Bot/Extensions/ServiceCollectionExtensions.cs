using Microsoft.Extensions.DependencyInjection;
using Weather_Bot.Contract;
using Weather_Bot.Service;

namespace Weather_Bot;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWeatherBotServices(this IServiceCollection services)
    {
        services.AddHttpClient<IWeatherData, WindyService>();

        services.AddSingleton<IMessageSender>(sp =>
        {
            var weatherData = sp.GetRequiredService<IWeatherData>();
            var token = GetRequiredEnv("TELEGRAM_BOT_TOKEN");
            return new NotificationService(token, weatherData);
        });

        return services;
    }

    private static string GetRequiredEnv(string name)
    {
        var value = Environment.GetEnvironmentVariable(name);
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"Переменная окружения {name} не задана. Установите её в .env или в окружении.");
        }

        return value;
    }
}

