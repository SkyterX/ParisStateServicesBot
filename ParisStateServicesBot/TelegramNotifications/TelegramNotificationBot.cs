using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ParisStateServicesBot.TelegramNotifications
{
    public class TelegramNotificationBot : IDisposable
    {
        private TelegramSubscriptionDB SubscriptionDB { get; }
        private TelegramBotClient Bot { get; }

        private TelegramNotificationBot(TelegramSubscriptionDB subscriptionDB, TelegramConfig configuration)
        {
            SubscriptionDB = subscriptionDB;
            Bot = new TelegramBotClient(configuration.BotToken);
            Bot.OnMessage += HandleMessage;

            Bot.StartReceiving();
        }

        private async void HandleMessage(object sender, MessageEventArgs args)
        {
            var message = args.Message;

            if (message?.Type != MessageType.TextMessage) return;
            if (message.Text.StartsWith("/subscribe"))
            {
                await SubscriptionDB.CreateAsync(new TelegramSubscription {ChatId = message.Chat.Id}).ConfigureAwait(false);
                await Bot.SendTextMessageAsync(message.Chat.Id, "Welcome! You subscribed to the notifications.");
            }
            else
            {
                var usage = @"Usage:
/subscribe - subscribe to notifications
";

                await Bot.SendTextMessageAsync(message.Chat.Id, usage);
            }
        }

        public async Task NotifyAsync(string bookingStatus)
        {
            var subscriptions = await SubscriptionDB.LoadAllAsync();
            foreach (var subscription in subscriptions)
            {
                await Bot.SendTextMessageAsync(subscription.ChatId, bookingStatus).ConfigureAwait(false);
            }
        }

        public static async Task<TelegramNotificationBot> CreateAsync(TelegramSubscriptionDB subscriptionDB, TelegramConfigurationDB configDB)
            => new TelegramNotificationBot(subscriptionDB, await configDB.LoadAsync().ConfigureAwait(false));

        public void Dispose()
        {
            Bot.StopReceiving();
        }
    }
}