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
            using (var bot = await factory.Get<Task<TelegramNotificationBot>>())
            using (new NotificationTask(factory, bot).Run())
            using (new VerificationTask(factory, bot).Run())
            {
                Console.ReadKey();
            }
        }
    }
}