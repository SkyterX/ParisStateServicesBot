using System;
using System.Threading.Tasks;
using ParisStateServicesBot.PeriodicTasks;
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
            {
                await factory.Get<TelegramNotificationBot>().StartAsync();
                factory.Get<NotificationTask>().Run();
                factory.Get<VerificationTask>().Run();
                Console.ReadKey();
            }
        }
    }
}