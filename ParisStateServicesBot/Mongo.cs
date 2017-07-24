using MongoDB.Driver;

namespace ParisStateServicesBot
{
    public class Mongo
    {
        public MongoClient Client => new MongoClient("mongodb://localhost:27017");
        public IMongoDatabase Database => Client.GetDatabase("paris-ss-bot");
    }
}