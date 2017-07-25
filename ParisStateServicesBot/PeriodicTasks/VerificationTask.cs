using System;
using System.Threading.Tasks;
using ParisStateServicesBot.TelegramNotifications;

namespace ParisStateServicesBot.PeriodicTasks
{
    public class VerificationTask : PeriodicAsyncTask
    {
        private TelegramNotificationsDB NotificationsDB { get; }

        public VerificationTask(TelegramNotificationBot bot, TelegramNotificationsDB notificationsDB)
            : base(bot)
        {
            NotificationsDB = notificationsDB;
        }

        protected override async Task RunAsync()
        {
            var lastNotification = await NotificationsDB.LoadLatestAsync().ConfigureAwait(false);
            if (lastNotification == null)
                return;
            if (lastNotification.Timestamp < DateTime.UtcNow - DefaultDelay)
                await Bot.NotifyErrorAsync(new Exception($"Last notification was at {lastNotification.Timestamp}")).ConfigureAwait(false);
        }

        protected override TimeSpan DefaultDelay => TimeSpan.FromMinutes(15);
    }
}