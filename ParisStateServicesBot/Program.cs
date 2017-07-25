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
            using (await factory.Get<TelegramNotificationBot>().StartAsync())
            using (factory.Get<NotificationTask>().Run())
            using (factory.Get<VerificationTask>().Run())
            {
                Console.ReadKey();
            }
        }
    }
}