using Quartz;
using Weather_Bot.Contract;

namespace Weather_Bot.BackgroungTimeWorker;

public class EveningTaskJob : IJob
{
    private readonly  IMeteoService _meteoService;
    
    public EveningTaskJob(IMeteoService meteoService)
    {
        _meteoService = meteoService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        await _messageSender.SendEveningReportAsync();
    }
}