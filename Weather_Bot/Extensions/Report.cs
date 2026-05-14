using System.Text;
using System.Text.Json;
using Weather_Bot.Contract;
using Weather_Bot.DTOs;
using Weather_Bot.Enum;

namespace Weather_Bot;

public class Report : IReportWeather
{
    public string FormatReport(WindyWeatherResponse data, WeatherReportType type)
    {
        var sb = new StringBuilder("Пролив Дрейка:\n");
        
        if (data.ExtraData.TryGetValue("wind_u-surface", out var uObj) && 
            data.ExtraData.TryGetValue("wind_v-surface", out var vObj))
        {
            var speed = CalculateSpeed(uObj, vObj);
            sb.AppendLine($"💨 Ветер: {speed * 3.6:F0} км/ч");
        }

        if (data.ExtraData.TryGetValue("windGust-surface", out var gustObj))
        {
            var gust = ((JsonElement)gustObj)[0].GetSingle();
            sb.AppendLine($"🌪 Порывы: {gust * 3.6:F0} км/ч");
        }

        if (data.ExtraData.TryGetValue("waves-surface", out var wavesObj))
        {
            var waveHeight = ((JsonElement)wavesObj)[0].GetSingle();
            sb.AppendLine($"🌊 Высота волн: {waveHeight:F1} м");
        }

        return sb.ToString();
    }
    
    public  double CalculateSpeed(object uObj, object vObj)
    {
        var u = ((JsonElement)uObj)[0].GetSingle();
        var v = ((JsonElement)vObj)[0].GetSingle();
        return Math.Sqrt(u * u + v * v);
    }   
}