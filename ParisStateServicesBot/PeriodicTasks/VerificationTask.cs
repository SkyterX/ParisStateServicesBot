using System;
using System.Threading.Tasks;
using ParisStateServicesBot.TelegramNotifications;

namespace ParisStateServicesBot.PeriodicTasks
{
    public class VerificationTask : PeriodicAsyncTask
    {
        public VerificationTask(TheFactory factory, TelegramNotificationBot bot) : base(factory, bot)
        {
        }

        protected override async Task RunAsync(TheFactory factory, TelegramNotificationBot bot)
        {
            var notificationsDB = factory.Get<TelegramNotificationsDB>();
            var lastNotification = await notificationsDB.LoadLatestAsync().ConfigureAwait(false);
            if (lastNotification == null)
                return;
            if (lastNotification.Timestamp < DateTime.UtcNow - DefaultDelay)
                await bot.NotifyErrorAsync(new Exception($"Last notification was at {lastNotification.Timestamp}")).ConfigureAwait(false);
        }

        protected override TimeSpan DefaultDelay => TimeSpan.FromMinutes(15);
    }
}