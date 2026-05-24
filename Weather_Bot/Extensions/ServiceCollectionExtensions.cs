using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Telegram.Bot;
using Weather_Bot.BackgroungTimeWorker;
using Weather_Bot.Contract;
using Weather_Bot.Service;
using Weather_Bot.Handlers;

namespace Weather_Bot;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWeatherBotServices(this IServiceCollection services)
    {
        services.AddSingleton<ITelegramService, TelegramService>();
        
        services.AddSingleton<HandlerUpdateAsync>();
        services.AddSingleton<HandlerErrorAsync>();

        services.AddSingleton<ITelegramBotClient>(sp =>
        {
            var token = GetRequiredEnv("TELEGRAM_BOT_TOKEN");
            return new TelegramBotClient(token);
        });

        services.AddQuartz(q =>
        {
            q.UseMicrosoftDependencyInjectionJobFactory();

            var jobKey = new JobKey("EveningTaskJob");

            q.AddJob<EveningTaskJob>(opts => opts.WithIdentity(jobKey));

            q.AddTrigger(opts => opts
                .ForJob(jobKey)
                .WithIdentity("EveningWeatherTrigger")
                .WithCronSchedule("0 0 20 * * ?", x => x
                    .InTimeZone(TimeZoneInfo.FindSystemTimeZoneById("Europe/Warsaw"))
                ));
        });

        services.AddQuartzHostedService(opt => { opt.WaitForJobsToComplete = true; });

        return services;
    }

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