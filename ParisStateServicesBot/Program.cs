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
                var task = Task.Run(() => RunNotifier(factory, bot, token.Token), token.Token);
                Console.ReadKey();
                token.Cancel();
                await task;
            }
        }

        private static async Task RunNotifier(TheFactory factory, TelegramNotificationBot bot, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var delay = TimeSpan.FromMinutes(5);
                var stopwatch = Stopwatch.StartNew();
                try
                {
                    var bookingStatus = factory.BookingStatusLoader.GetBookingStatus();
                    if (!bookingStatus.Contains("Vérification de disponibilité"))
                        await bot.NotifyAsync(bookingStatus);
                }
                catch (Exception e)
                {
                    delay = TimeSpan.FromSeconds(30);
                    await bot.NotifyErrorAsync(e);
                    Console.WriteLine(e);
                }
                stopwatch.Stop();
                if (stopwatch.Elapsed < delay)
                    await Task.Delay(delay - stopwatch.Elapsed, token);
            }
        }
    }
}