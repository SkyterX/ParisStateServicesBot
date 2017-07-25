using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using ParisStateServicesBot.PeriodicTasks;
using ParisStateServicesBot.TelegramNotifications;

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
            {
                var obj = (T) value.Value;
                if (obj is IDisposable)
                    enqueuedForDispose.GetOrAdd(typeof(T), type =>
                    {
                        disposeQueue.Add((IDisposable) obj);
                        return true;
                    });
                return obj;
            }
            throw new Exception($"Type {typeof(T).Name} has no implementations registered");
        }

        private void Register<T>(Func<T> factory)
        {
            cache.Add(typeof(T), new Lazy<object>(() => factory()));
        }

        private void Register<TArg, T>(Func<TArg, T> factory)
        {
            cache.Add(typeof(T), new Lazy<object>(() => factory(Get<TArg>())));
        }

        private void Register<TArg1, TArg2, T>(Func<TArg1, TArg2, T> factory)
        {
            cache.Add(typeof(T), new Lazy<object>(() => factory(Get<TArg1>(), Get<TArg2>())));
        }

        private void Register<TArg1, TArg2, TArg3, T>(Func<TArg1, TArg2, TArg3, T> factory)
        {
            cache.Add(typeof(T), new Lazy<object>(() => factory(Get<TArg1>(), Get<TArg2>(), Get<TArg3>())));
        }

        public void Dispose()
        {
            foreach (var disposable in disposeQueue.Reverse())
            {
                disposable.Dispose();
            }
        }

        private readonly Dictionary<Type, Lazy<object>> cache = new Dictionary<Type, Lazy<object>>();
        private readonly ConcurrentDictionary<Type, bool> enqueuedForDispose = new ConcurrentDictionary<Type, bool>();
        private readonly IList<IDisposable> disposeQueue = new List<IDisposable>();
    }
}