using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using ParisStateServicesBot.PeriodicTasks;
using ParisStateServicesBot.TelegramNotifications;
using ParisStateServicesBot.Util;

namespace ParisStateServicesBot
{
    public class TheFactory : IDisposable
    {
        public TheFactory()
        {
            Register<IWebDriver>(() => new ChromeDriver());
            Register((IWebDriver driver) => new BookingStatusLoader(driver));

            Register(() => new Mongo());
            Register((Mongo mongo) => new TelegramSubscriptionDB(mongo));
            Register((Mongo mongo) => new TelegramConfigurationDB(mongo));
            Register((Mongo mongo) => new TelegramNotificationsDB(mongo));
            Register((TelegramSubscriptionDB x,
                    TelegramNotificationsDB y,
                    TelegramConfigurationDB z)
                => new TelegramNotificationBot(x, y, z));

            Register((TelegramNotificationBot bot, TelegramNotificationsDB nDB) => new VerificationTask(bot, nDB));
            Register((TelegramNotificationBot bot, BookingStatusLoader loader) => new NotificationTask(bot, loader));
        }

        public T Get<T>()
        {
            if (cache.TryGetValue(typeof(T), out var value))
                return (T) value.Value;
            throw new Exception($"Type {typeof(T).Name} has no implementations registered");
        }

        private void Register<T>(Func<T> factory)
        {
            cache.Add(typeof(T), new LazyDisposable<object>(() => factory()));
        }

        private void Register<TArg, T>(Func<TArg, T> factory)
        {
            cache.Add(typeof(T), new LazyDisposable<object>(() => factory(Get<TArg>())));
        }

        private void Register<TArg1, TArg2, T>(Func<TArg1, TArg2, T> factory)
        {
            cache.Add(typeof(T), new LazyDisposable<object>(() => factory(Get<TArg1>(), Get<TArg2>())));
        }

        private void Register<TArg1, TArg2, TArg3, T>(Func<TArg1, TArg2, TArg3, T> factory)
        {
            cache.Add(typeof(T), new LazyDisposable<object>(() => factory(Get<TArg1>(), Get<TArg2>(), Get<TArg3>())));
        }

        public void Dispose()
        {
            foreach (var lazyDisposable in cache.Values)
            {
                lazyDisposable.Dispose();
            }
        }

        private readonly Dictionary<Type, LazyDisposable<object>> cache = new Dictionary<Type, LazyDisposable<object>>();
    }
}