using Microsoft.Extensions.Options;
using Quartz;
using Weather_Bot.Configuration;
using Weather_Bot.Contract;

namespace Weather_Bot.BackgroungTimeWorker;

public class EveningTaskJob : IJob
{
    private readonly IMeteoService _meteoService;
    private readonly BotConfiguration _config;
    
    public EveningTaskJob(IMeteoService meteoService,  IOptions<BotConfiguration> botConfiguration)
    {
        _config = botConfiguration.Value;
        _meteoService = meteoService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        await _meteoService.GetWindAsync();
    }
}