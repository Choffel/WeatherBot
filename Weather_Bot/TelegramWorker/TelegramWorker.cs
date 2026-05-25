using Microsoft.Extensions.Hosting;
using Weather_Bot.Contract;

namespace Weather_Bot.TelegramWorker;

public class TelegramWorker : BackgroundService
{
    private readonly ITelegramService _telegramService;

    public TelegramWorker(ITelegramService telegramService)
    {
        _telegramService = telegramService;
    }
    
    
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
       
        _ = Task.Run(async () =>
        {
            try
            {
                Console.WriteLine("🚀 [TelegramWorker] Поток воркера запущен. Активируем Long Polling...");
                
                await _telegramService.StartAsync();
                
                Console.WriteLine("✅ [TelegramWorker] Бот-клиент успешно вызвал StartReceiving!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 [TelegramWorker] КРИТИЧЕСКАЯ ОШИБКА ПРИ СТАРТЕ БОТА: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }, stoppingToken);

      
        return Task.CompletedTask;
    }
}