using System.Threading.Tasks;
using MongoDB.Driver;

namespace ParisStateServicesBot.TelegramNotifications
{
    public class TelegramConfigurationDB
    {
        private readonly Mongo mongo;
        private const string CollectionName = "telegram-config";
        private const string ConfigId = "telegram-config";

        public TelegramConfigurationDB(Mongo mongo)
        {
            this.mongo = mongo;
        }

        public Task CreateAsync(TelegramConfig config)
        {
            config.Id = ConfigId;
            return Collection.ReplaceOneAsync(dbConfig => dbConfig.Id == ConfigId, config, new UpdateOptions {IsUpsert = true});
        }

        public Task<TelegramConfig> LoadAsync() => Collection.Find(dbConfig => dbConfig.Id == ConfigId).SingleOrDefaultAsync();

        private IMongoCollection<TelegramConfig> Collection => mongo.Database.GetCollection<TelegramConfig>(CollectionName);
    }
}