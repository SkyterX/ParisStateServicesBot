using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace ParisStateServicesBot.TelegramNotifications
{
    public class TelegramNotificationsDB
    {
        private readonly Mongo mongo;
        private const string CollectionName = "telegram-notifications";

        public TelegramNotificationsDB(Mongo mongo)
        {
            this.mongo = mongo;
        }

        public Task InsertAsync(TelegramNotification notification)
        {
            notification.Timestamp = DateTime.UtcNow;
            return Collection.InsertOneAsync(notification);
        }

        public Task<List<TelegramNotification>> LoadAllAsync() => Collection.Find(_ => true).ToListAsync();

        public Task<TelegramNotification> LoadLatestAsync()
            => Collection
                .Find(_ => true)
                .SortByDescending(x => x.Timestamp)
                .FirstOrDefaultAsync();

        private IMongoCollection<TelegramNotification> Collection => mongo.Database.GetCollection<TelegramNotification>(CollectionName);
    }
}