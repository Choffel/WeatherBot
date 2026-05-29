using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Quartz;
using Telegram.Bot;
using Weather_Bot.BackgroungTimeWorker;
using Weather_Bot.Configuration;
using Weather_Bot.Contract;
using Weather_Bot.Contract.Lublin;
using Weather_Bot.Contract.NemoPoint;
using Weather_Bot.Service;
using Weather_Bot.Handlers;

namespace Weather_Bot.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWeatherBotServices(this IServiceCollection services, IConfiguration configuration)
    {
        
        services.Configure<BotConfiguration>(configuration.GetSection(BotConfiguration.SectionName));
        
        
        services.AddHttpClient();
        
       
        services.AddSingleton<OpenMeteoService>();
        services.AddSingleton<IMeteoService>(sp => sp.GetRequiredService<OpenMeteoService>());
        services.AddSingleton<ILublinWeather>(sp => sp.GetRequiredService<OpenMeteoService>());
        services.AddSingleton<INemoPoint>(sp => sp.GetRequiredService<OpenMeteoService>());
        
        
        services.AddTransient<HandlerUpdateAsync>();
        services.AddTransient<HandlerErrorAsync>();

        
        services.AddSingleton<TelegramService>();
        services.AddSingleton<ITelegramService>(sp => sp.GetRequiredService<TelegramService>());

        
        services.AddSingleton<ITelegramBotClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<BotConfiguration>>();
            var token = options.Value.TELEGRAM_BOT_TOKEN;
    
            if (string.IsNullOrWhiteSpace(token))
                throw new InvalidOperationException(
                    "❌ Ошибка конфигурации: TELEGRAM_BOT_TOKEN не задан. " +
                    "Проверьте файл .env в корне проекта.");
    
            // Создаем клиент и даем ему 1 минуту на ожидание Long Polling
            var httpClient = new HttpClient { Timeout = TimeSpan.FromMinutes(1) };
            return new TelegramBotClient(token, httpClient); 
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