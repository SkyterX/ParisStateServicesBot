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
        private TelegramConfigurationDB ConfigDB { get; }
        private TelegramSubscriptionDB SubscriptionDB { get; }
        private TelegramNotificationsDB NotificationsDB { get; }
        private TelegramBotClient Bot { get; set; }

        public TelegramNotificationBot(
            TelegramSubscriptionDB subscriptionDB,
            TelegramNotificationsDB notificationsDB,
            TelegramConfigurationDB configDB)
        {
            NotificationsDB = notificationsDB;
            ConfigDB = configDB;
            SubscriptionDB = subscriptionDB;
        }

        public async Task StartAsync()
        {
            var initialConfiguration = await ConfigDB.LoadAsync().ConfigureAwait(false);
            Bot = new TelegramBotClient(initialConfiguration.BotToken);
            Bot.OnMessage += HandleMessage;
            Bot.StartReceiving();
        }

        private async void HandleMessage(object sender, MessageEventArgs args)
        {
            try
            {
                var message = args.Message;
                if (message?.Type != MessageType.TextMessage) return;
                var response = await HandleMessageAsync(message);
                await Bot.SendTextMessageAsync(message.Chat.Id, response, ParseMode.Markdown).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                await NotifyErrorAsync(e);
            }
        }

        private async Task<string> HandleMessageAsync(Message message)
        {
            if (message.Text.StartsWith("/subscribe"))
            {
                await SubscriptionDB.CreateAsync(new TelegramSubscription {ChatId = message.Chat.Id}).ConfigureAwait(false);
                return "Welcome! You subscribed to the notifications.";
            }
            if (message.Text.StartsWith("/ping"))
                return "Pong!";
            if (message.Text.StartsWith("/last"))
            {
                var lastMessage = await NotificationsDB.LoadLatestAsync();
                if (lastMessage == null)
                {
                    return "No messages yet.";
                }

                var lastMessageTime = lastMessage.Timestamp + (message.Date - DateTime.UtcNow);
                var formattedStatus = FormatBookingStatus(lastMessage.Status);
                return $"Latest message was at {lastMessageTime:T}\n{formattedStatus}";
            }

            return @"Usage:
/subscribe - subscribe to notifications
";
        }

        public async Task NotifyAsync(BookingStatus bookingStatus)
        {
            var lastNotification = await NotificationsDB.LoadLatestAsync().ConfigureAwait(false);
            await NotificationsDB.InsertAsync(new TelegramNotification {Status = bookingStatus}).ConfigureAwait(false);
            if (bookingStatus.Title == lastNotification?.Status?.Title)
                return;

            var subscriptions = await SubscriptionDB.LoadAllAsync();
            foreach (var subscription in subscriptions)
            {
                await Bot.SendTextMessageAsync(subscription.ChatId, FormatBookingStatus(bookingStatus), ParseMode.Markdown).ConfigureAwait(false);
            }
        }

        public string FormatBookingStatus(BookingStatus x)
        {
            return $"*{x.Title}*\n{x.Description}";
        }

        public async Task NotifyErrorAsync(Exception e)
        {
            var config = await ConfigDB.LoadAsync().ConfigureAwait(false);
            await Bot.SendTextMessageAsync(config.OwnerChatId, "Error occured :" + e);
        }

        public void Dispose()
        {
            Bot.StopReceiving();
        }
    }
}