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
            Factory.Get<TelegramConfigurationDB>().CreateAsync(config).Wait();
        }
    }
}