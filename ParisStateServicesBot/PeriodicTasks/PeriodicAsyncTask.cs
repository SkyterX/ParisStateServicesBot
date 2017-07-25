using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using ParisStateServicesBot.TelegramNotifications;

namespace ParisStateServicesBot.PeriodicTasks
{
    public abstract class PeriodicAsyncTask : IDisposable
    {
        private CancellationTokenSource TokenSource { get; }
        private Thread RunnerThread { get; }

        protected PeriodicAsyncTask(TheFactory factory, TelegramNotificationBot bot)
        {
            TokenSource = new CancellationTokenSource();
            var token = TokenSource.Token;
            RunnerThread = new Thread(() =>
            {
                while (!token.IsCancellationRequested)
                {
                    Task.Run(() => TryRunAsync(factory, bot, token), token).Wait(token);
                }
            });
        }

        public PeriodicAsyncTask Run()
        {
            RunnerThread.Start();
            return this;
        }

        public void Cancel(TimeSpan timeout)
        {
            if (TokenSource.IsCancellationRequested)
                return;
            TokenSource.Cancel();
            RunnerThread.Join(timeout);
        }

        public void Dispose()
        {
            Cancel(DelayAfterError);
            TokenSource?.Dispose();
        }

        private async Task TryRunAsync(TheFactory factory, TelegramNotificationBot bot, CancellationToken token)
        {
            var delay = DefaultDelay;
            var stopwatch = Stopwatch.StartNew();
            try
            {
                await RunAsync(factory, bot).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                delay = DelayAfterError;
                await bot.NotifyErrorAsync(e).ConfigureAwait(false);
                Console.WriteLine(e);
            }
            stopwatch.Stop();
            if (stopwatch.Elapsed < delay)
                await Task.Delay(delay - stopwatch.Elapsed, token).ConfigureAwait(false);
        }

        protected abstract Task RunAsync(TheFactory factory, TelegramNotificationBot bot);

        protected abstract TimeSpan DefaultDelay { get; }
        protected virtual TimeSpan DelayAfterError => TimeSpan.FromSeconds(30);
    }
}