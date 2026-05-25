using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Weather_Bot.Configuration;
using Weather_Bot.Extensions; 
using Weather_Bot.Contract;

try
{
    Console.WriteLine("[DEBUG LOG] 1. Вход в точку старта приложения...");
    EnvironmentSetup.LoadDotEnv();

    var builder = Host.CreateApplicationBuilder(args);
    builder.Services.AddWeatherBotServices(builder.Configuration);

    using IHost host = builder.Build();
    Console.WriteLine("[DEBUG LOG] 5. DI-контейнер успешно собран.");

    
    Console.WriteLine("🛰️ [⚠️ СУПЕР-ТЕСТ] Принудительный ручной запуск Telegram Long Polling...");
    var telegramService = host.Services.GetRequiredService<ITelegramService>();
    await telegramService.StartAsync();
    Console.WriteLine("🔥 [⚠️ СУПЕР-ТЕСТ] StartAsync успешно выполнен! Бот должен слушать сервера Telegram.");

    Console.WriteLine("🤖 [Host] Запуск основного хоста приложений...");
    await host.RunAsync();
}
catch (Exception ex)
{
    Console.WriteLine($"💥 КРИТИЧЕСКАЯ ОШИБКА ПРИ СТАРТЕ ХОСТА: {ex.Message}");
    Console.WriteLine(ex.StackTrace);
    Environment.Exit(1);
}