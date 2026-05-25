using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Weather_Bot.Configuration;
using Weather_Bot.Contract;
using Weather_Bot;
using Weather_Bot.Extensions; // Добавлено для использования AddWeatherBotServices

// Загружаем переменные окружения из файла .env
EnvironmentSetup.LoadDotEnv();

var builder = Host.CreateApplicationBuilder(args);

// Создаем конфигурацию
var configuration = new ConfigurationBuilder()
    .AddEnvironmentVariables()
    .Build();

// Регистрируем все сервисы приложения
builder.Services.AddWeatherBotServices(configuration);

using IHost host = builder.Build();

try
{
    var telegramService = host.Services.GetRequiredService<ITelegramService>();

    // Запускаем приложение
    Console.WriteLine("🤖 Запуск Weather Bot...");
    
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
