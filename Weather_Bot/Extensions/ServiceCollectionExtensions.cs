using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Quartz;
using Telegram.Bot;
using Weather_Bot.BackgroungTimeWorker;
using Weather_Bot.Configuration;
using Weather_Bot.Contract;
using Weather_Bot.Contract.Lublin;
using Weather_Bot.Service;
using Weather_Bot.Handlers;

namespace Weather_Bot.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWeatherBotServices(this IServiceCollection services, IConfiguration configuration)
    {
        
        // register bot configuration
        services.Configure<BotConfiguration>(configuration.GetSection(BotConfiguration.SectionName));
        
        // регистрируем сервисы погоды
        services.AddHttpClient<OpenMeteoService>();
        
        services.AddSingleton(sp => sp.GetRequiredService<OpenMeteoService>());
        
        services.AddSingleton<IMeteoService>(sp => sp.GetRequiredService<OpenMeteoService>());
        services.AddSingleton<ILublinWeather>(sp => sp.GetRequiredService<OpenMeteoService>());
        
        
        services.AddSingleton<ITelegramService, TelegramService>();
        
        services.AddSingleton<HandlerUpdateAsync>();
        services.AddSingleton<HandlerErrorAsync>();

        services.AddSingleton<ITelegramBotClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<BotConfiguration>>();
            var token = options.Value.TELEGRAM_BOT_TOKEN;
            
            if (string.IsNullOrWhiteSpace(token))
                throw new InvalidOperationException(
                    $"❌ Ошибка конфигурации: TELEGRAM_BOT_TOKEN не задан. " +
                    $"Проверьте файл .env в корне проекта.");
            
            return new TelegramBotClient(token, httpClient: null);
        });

        services.AddQuartz(q =>
        {
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
}