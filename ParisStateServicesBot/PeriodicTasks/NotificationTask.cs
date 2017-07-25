using System;
using System.Threading.Tasks;
using ParisStateServicesBot.TelegramNotifications;

namespace ParisStateServicesBot.PeriodicTasks
{
    public class NotificationTask : PeriodicAsyncTask
    {
        private BookingStatusLoader BookingStatusLoader { get; }

        public NotificationTask(TelegramNotificationBot bot, BookingStatusLoader bookingStatusLoader)
            : base(bot)
        {
            BookingStatusLoader = bookingStatusLoader;
        }

        protected override async Task RunAsync()
        {
            var bookingStatus = BookingStatusLoader.GetBookingStatus();
            await Bot.NotifyAsync(bookingStatus).ConfigureAwait(false);
        }

        protected override TimeSpan DefaultDelay => TimeSpan.FromMinutes(5);
    }
}