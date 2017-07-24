using System.Threading.Tasks;
using Telegram.Bot;

namespace ParisStateServicesBot.TelegramNotifications
{
    public class TelegramNotificationBot
    {
        private TelegramBotClient Bot { get; }

        public TelegramNotificationBot(TelegramConfig configuration)
        {
            Bot = new TelegramBotClient(configuration.BotToken);
        }

        public static async Task<TelegramNotificationBot> CreateAsync(TelegramConfigurationDB db) => new TelegramNotificationBot(await db.LoadAsync());
    }
}