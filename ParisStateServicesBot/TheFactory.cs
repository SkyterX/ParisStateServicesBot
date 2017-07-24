using System;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using ParisStateServicesBot.TelegramNotifications;

namespace ParisStateServicesBot
{
    public class TheFactory : IDisposable
    {
        private readonly Lazy<Mongo> mongo;
        private readonly Lazy<TelegramConfigurationDB> telegramConfigurationDB;
        private readonly Lazy<Task<TelegramNotificationBot>> telegramNotificationBot;
        private readonly Lazy<IWebDriver> webDriver;
        private Lazy<BookingStatusLoader> bookingStatusLoader;

        public TheFactory()
        {
            webDriver = new Lazy<IWebDriver>(() => new ChromeDriver());
            bookingStatusLoader = new Lazy<BookingStatusLoader>(() => new BookingStatusLoader(WebDriver));

            mongo = new Lazy<Mongo>(() => new Mongo());
            telegramConfigurationDB = new Lazy<TelegramConfigurationDB>(() => new TelegramConfigurationDB(Mongo));
            telegramNotificationBot = new Lazy<Task<TelegramNotificationBot>>(() => TelegramNotificationBot.CreateAsync(TelegramConfigurationDB));
        }

        public void Dispose()
        {
            if (webDriver.IsValueCreated)
                webDriver.Value?.Dispose();
            if (bookingStatusLoader.IsValueCreated)
                bookingStatusLoader.Value?.Dispose();
        }

        public IWebDriver WebDriver => webDriver.Value;
        public BookingStatusLoader BookingStatusLoader => bookingStatusLoader.Value;

        public Mongo Mongo => mongo.Value;
        public TelegramConfigurationDB TelegramConfigurationDB => telegramConfigurationDB.Value;
        public Task<TelegramNotificationBot> NotificationBot => telegramNotificationBot.Value;
    }
}