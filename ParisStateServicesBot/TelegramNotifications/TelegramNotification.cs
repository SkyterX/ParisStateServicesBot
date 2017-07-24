using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ParisStateServicesBot.TelegramNotifications
{
    public class TelegramNotification
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
    }
}