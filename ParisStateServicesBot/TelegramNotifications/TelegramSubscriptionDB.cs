using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace ParisStateServicesBot.TelegramNotifications
{
    public class TelegramSubscriptionDB
    {
        private readonly Mongo mongo;
        private const string CollectionName = "telegram-subscriptions";

        public TelegramSubscriptionDB(Mongo mongo)
        {
            this.mongo = mongo;
        }

        public Task CreateAsync(TelegramSubscription subscription)
        {
            return Collection.ReplaceOneAsync(dbSubscription => dbSubscription.ChatId == subscription.ChatId, subscription, new UpdateOptions { IsUpsert = true });
        }

        public Task<List<TelegramSubscription>> LoadAllAsync() => Collection.Find(_ => true).ToListAsync();

        private IMongoCollection<TelegramSubscription> Collection => mongo.Database.GetCollection<TelegramSubscription>(CollectionName);
    }
}