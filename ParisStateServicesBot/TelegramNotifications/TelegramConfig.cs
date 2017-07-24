using MongoDB.Bson.Serialization.Attributes;

namespace ParisStateServicesBot.TelegramNotifications
{
    public class TelegramConfig
    {
        [BsonId]
        public string Id { get; set; }

        public string BotToken { get; set; }
    }
}