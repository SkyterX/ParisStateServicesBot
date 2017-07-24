using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using ParisStateServicesBot.TelegramNotifications;

namespace ParisStateServicesBot
{
    class Program
    {
        public static void Main(string[] args)
        {
            RunAsync().Wait();
        }

        private static async Task RunAsync()
        {
            using (var factory = new TheFactory())
            using (var bot = await factory.NotificationBot)
            {
                var token = new CancellationTokenSource();
                Task.Run(() => RunNotifier(factory, bot, token.Token), token.Token);
                Console.ReadKey();
                token.Cancel();
            }
        }

        private static async Task RunNotifier(TheFactory factory, TelegramNotificationBot bot, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var stopwatch = Stopwatch.StartNew();
                var bookingStatus = factory.BookingStatusLoader.GetBookingStatus();

                if(!bookingStatus.Contains("Vérification de disponibilité"))
                    await bot.NotifyAsync(bookingStatus);

                if (stopwatch.Elapsed.TotalMinutes < 5)
                    await Task.Delay(TimeSpan.FromMinutes(5) - stopwatch.Elapsed, token);
            }
        }
    }
}