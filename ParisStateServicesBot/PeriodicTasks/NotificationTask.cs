using System;
using System.Threading.Tasks;
using ParisStateServicesBot.TelegramNotifications;

namespace ParisStateServicesBot.PeriodicTasks
{
    public class NotificationTask : PeriodicAsyncTask
    {
        public NotificationTask(TheFactory factory, TelegramNotificationBot bot) : base(factory, bot)
        {
        }

        protected override async Task RunAsync(TheFactory factory, TelegramNotificationBot bot)
        {
            var bookingStatus = factory.Get<BookingStatusLoader>().GetBookingStatus();
            await bot.NotifyAsync(bookingStatus).ConfigureAwait(false);
        }

        protected override TimeSpan DefaultDelay => TimeSpan.FromMinutes(5);
    }
}