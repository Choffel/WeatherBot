using Telegram.Bot.Types.ReplyMarkups;

namespace Weather_Bot;

public static class ReplyMarkups
{
    //creating buttons 
    public static InlineKeyboardMarkup GetDrakeKeyboard() =>
        new(new[]
        {
            new[] 
            {
                InlineKeyboardButton.WithCallbackData("💨 Ветер", "report_wind"),
            },
            new[] { InlineKeyboardButton.WithCallbackData("📊 Полная сводка", "report_full") }
        });
    //
    // public static InlineKeyboardMarkup GetAntarcticaKeyboard() =>
    //     new(new[] { });
}