using System;
using System.Threading.Tasks;

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
                var bookingStatus = factory.BookingStatusLoader.GetBookingStatus();
                await bot.NotifyAsync(bookingStatus);
                Console.ReadKey();
            }
        }
    }
}