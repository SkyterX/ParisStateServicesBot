using MongoDB.Bson.Serialization.Attributes;

namespace ParisStateServicesBot.TelegramNotifications
{
    public class TelegramSubscription
    {
        [BsonId]
        public long ChatId { get; set; }
    }
}