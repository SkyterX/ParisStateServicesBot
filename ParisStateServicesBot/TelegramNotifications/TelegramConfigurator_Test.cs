using NUnit.Framework;

namespace ParisStateServicesBot.TelegramNotifications
{
    [TestFixture]
    public class TelegramConfigurator_Test : TestBase
    {
        [Test]
        public void CreateConfiguration()
        {
            var config = new TelegramConfig
            {
                BotToken = null
            };
            Factory.TelegramConfigurationDB.CreateAsync(config).Wait();
        }
    }
}