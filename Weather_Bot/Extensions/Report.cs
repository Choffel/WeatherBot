using System.Text;
using System.Text.Json;
using Weather_Bot.Contract;
using Weather_Bot.DTOs;

namespace Weather_Bot.Extensions;

public static class Report 
{
    public static string GetWeatherReport(WindyWeatherResponse data)
    {
        var sb = new StringBuilder();

        if (data.ExtraData.TryGetValue("wind_u-surface", out var windU) &&
            data.ExtraData.TryGetValue("wind_v-surface", out var windV))
        {
            var u = ((JsonElement)windU)[0].GetSingle();
            var v = ((JsonElement)windV)[0].GetSingle();
            var speed = Math.Sqrt(u * u + v * v);
            sb.AppendLine($"💨 Скорость ветра: {speed:F1} м/с");
        }

        if (data.ExtraData.TryGetValue("windGust-surface", out var gust))
        {
            var gustSpeed = ((JsonElement)gust)[0].GetSingle();
            sb.AppendLine($"🌪 Порывы ветра: {gustSpeed:F1} м/с");
        }

        return sb.ToString();
    }
}