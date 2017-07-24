using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;

namespace ParisStateServicesBot.TelegramNotifications
{
    public class TelegramNotificationBot : IDisposable
    {
        private TelegramConfigurationDB ConfigDB { get; }
        private TelegramSubscriptionDB SubscriptionDB { get; }
        private TelegramNotificationsDB NotificationsDB { get; }
        private TelegramBotClient Bot { get; }

        private TelegramNotificationBot(
            TelegramSubscriptionDB subscriptionDB,
            TelegramNotificationsDB notificationsDB,
            TelegramConfigurationDB configDB,
            TelegramConfig initialConfiguration)
        {
            NotificationsDB = notificationsDB;
            ConfigDB = configDB;
            SubscriptionDB = subscriptionDB;
            Bot = new TelegramBotClient(initialConfiguration.BotToken);
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
                await Bot.SendTextMessageAsync(message.Chat.Id, "Welcome! You subscribed to the notifications.").ConfigureAwait(false);
                return;
            }
            if (message.Text.StartsWith("/ping"))
            {
                await Bot.SendTextMessageAsync(message.Chat.Id, "Pong!").ConfigureAwait(false);
                return;
            }
            if (message.Text.StartsWith("/last"))
            {
                var lastMessage = await NotificationsDB.LoadLatestAsync();
                await Bot.SendTextMessageAsync(message.Chat.Id, $"Latest message was at {lastMessage.Timestamp:T}\n{lastMessage.Message}");
            }

            var usage = @"Usage:
/subscribe - subscribe to notifications
";

            await Bot.SendTextMessageAsync(message.Chat.Id, usage);
        }

        public async Task NotifyAsync(string bookingStatus)
        {
            await NotificationsDB.InsertAsync(new TelegramNotification {Message = bookingStatus}).ConfigureAwait(false);
            var subscriptions = await SubscriptionDB.LoadAllAsync();
            foreach (var subscription in subscriptions)
            {
                await Bot.SendTextMessageAsync(subscription.ChatId, bookingStatus).ConfigureAwait(false);
            }
        }

        public async Task NotifyErrorAsync(Exception e)
        {
            var config = await ConfigDB.LoadAsync().ConfigureAwait(false);
            await Bot.SendTextMessageAsync(config.OwnerChatId, "Error occured :" + e);
        }

        public static async Task<TelegramNotificationBot> CreateAsync(
            TelegramSubscriptionDB subscriptionDB,
            TelegramNotificationsDB notificationsDB,
            TelegramConfigurationDB configDB)
        {
            var initialConfiguration = await configDB.LoadAsync().ConfigureAwait(false);
            return new TelegramNotificationBot(subscriptionDB, notificationsDB, configDB, initialConfiguration);
        }

        public void Dispose()
        {
            Bot.StopReceiving();
        }
    }
}