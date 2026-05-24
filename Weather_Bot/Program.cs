using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Weather_Bot.Configuration;
using Weather_Bot.Contract;
using Weather_Bot.Service;
using Weather_Bot; // Добавлено для использования AddWeatherBotServices

// Загружаем переменные окружения из файла .env
EnvironmentSetup.LoadDotEnv();

var builder = Host.CreateApplicationBuilder(args);

// Регистрируем все сервисы приложения
builder.Services.AddWeatherBotServices();

using IHost host = builder.Build();

try
{
    var weatherData = host.Services.GetRequiredService<ITelegramBotClient>();
    var messageSender = host.Services.GetRequiredService<IMeteoService>();

    // Запускаем приложение
    Console.WriteLine("🤖 Запуск Weather Bot...");
    
    if(messageSender is ITelegramService telegramService)
        await telegramService.StartAsync();
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"❌ {ex.Message}");
    Environment.Exit(1);
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Непредвиденная ошибка: {ex.Message}");
    Environment.Exit(1);
}
