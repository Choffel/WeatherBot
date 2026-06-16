namespace Weather_Bot.Configuration
{
    public class BotConfiguration
    {
        public const string SectionName = "BotConfiguration";

        public string TELEGRAM_BOT_TOKEN { get; set; } = string.Empty;
        public long TELEGRAM_CHAT_ID { get; set; }
        public double LATITUDE { get; set; }
        public double LONGITUDE { get; set; }
        
        public double LUBLIN_LATITUDE { get; set; }  
        public double LUBLIN_LONGITUDE { get; set; } 
    }
}

