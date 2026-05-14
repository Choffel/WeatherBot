
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Weather_Bot.Contract;
using Weather_Bot.Service;

// Попытка загрузить .env (локально) в переменные окружения, чтобы не требовать сторонних зависимостей.
// Это простой парсер: поддерживает строки формата KEY=VALUE и игнорирует комментарии (#).

void LoadDotEnv()
{
    try
    {
        // Ищем .env в текущей директории и в каталоге приложения
        var candidates = new[] { Path.Combine(Directory.GetCurrentDirectory(), ".env"), Path.Combine(AppContext.BaseDirectory, ".env") };
        var path = candidates.FirstOrDefault(File.Exists);
        if (path == null) return;

        foreach (var raw in File.ReadAllLines(path))
        {
            var line = raw.Trim();
            if (string.IsNullOrEmpty(line) || line.StartsWith("#")) continue;
            var idx = line.IndexOf('=');
            if (idx <= 0) continue;
            var key = line.Substring(0, idx).Trim();
            var val = line.Substring(idx + 1).Trim();
            // Убираем кавычки вокруг значения, если есть
            if ((val.StartsWith("\"") && val.EndsWith("\"")) || (val.StartsWith("'") && val.EndsWith("'")))
            {
                val = val.Substring(1, val.Length - 2);
            }
            Environment.SetEnvironmentVariable(key, val);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("Не удалось загрузить .env: " + ex.Message);
    }
}

LoadDotEnv();

// 1. Инициализация построителя приложения
var builder = Host.CreateApplicationBuilder(args);

Weather_Bot.ServiceCollectionExtensions.AddWeatherBotServices(builder.Services);

// 4. Построение хоста
using IHost host = builder.Build();

// 5. Извлечение сервисов из контейнера
var windyService = host.Services.GetRequiredService<IWeatherData>();
var notificationService = host.Services.GetRequiredService<IMessageSender>();

Console.WriteLine("--- Система мониторинга запущена ---");

// 6. Логика работы
// Отправляем приветственное сообщение

await windyService.GetDrakeSummaryAsync();

await notificationService.SendAsync();

// Получаем сводку по Дрейку (метод внутри сам сходит в API)


// Запускаем прослушивание команд в Telegram
// Приводим к конкретному классу, так как StartAsync нет в интерфейсе IMessageSender
if (notificationService is NotificationService tgService)
{
    await tgService.StartAsync();
}
