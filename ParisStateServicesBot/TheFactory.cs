using System;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using ParisStateServicesBot.TelegramNotifications;
using ParisStateServicesBot.Util;

namespace ParisStateServicesBot
{
    public class TheFactory : IDisposable
    {
        private readonly Lazy<Mongo> mongo;
        private readonly Lazy<TelegramSubscriptionDB> telegramSubscriptionDB;
        private readonly Lazy<TelegramConfigurationDB> telegramConfigurationDB;
        private readonly Lazy<Task<TelegramNotificationBot>> telegramNotificationBot;

        private readonly LazyDisposable<IWebDriver> webDriver;
        private readonly LazyDisposable<BookingStatusLoader> bookingStatusLoader;

        public TheFactory()
        {
            webDriver = DisposableLazy<IWebDriver>(() => new ChromeDriver());
            bookingStatusLoader = DisposableLazy(() => new BookingStatusLoader(WebDriver));

            mongo = Lazy(() => new Mongo());
            telegramSubscriptionDB = Lazy(() => new TelegramSubscriptionDB(Mongo));
            telegramConfigurationDB = Lazy(() => new TelegramConfigurationDB(Mongo));
            telegramNotificationBot = DisposableLazy(() => TelegramNotificationBot.CreateAsync(TelegramSubscriptionDB, TelegramConfigurationDB));
        }

        private static Lazy<T> Lazy<T>(Func<T> factory) => new Lazy<T>(factory);
        private static LazyDisposable<T> DisposableLazy<T>(Func<T> factory) where T : IDisposable => new LazyDisposable<T>(factory);

        public void Dispose()
        {
            webDriver.Dispose();
            bookingStatusLoader.Dispose();
        }

        public IWebDriver WebDriver => webDriver.Value;
        public BookingStatusLoader BookingStatusLoader => bookingStatusLoader.Value;

        public Mongo Mongo => mongo.Value;
        public TelegramSubscriptionDB TelegramSubscriptionDB => telegramSubscriptionDB.Value;
        public TelegramConfigurationDB TelegramConfigurationDB => telegramConfigurationDB.Value;
        public Task<TelegramNotificationBot> NotificationBot => telegramNotificationBot.Value;
    }
}