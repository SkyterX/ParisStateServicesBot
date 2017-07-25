using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using ParisStateServicesBot.TelegramNotifications;

namespace ParisStateServicesBot.PeriodicTasks
{
    public abstract class PeriodicAsyncTask : IDisposable
    {
        protected TelegramNotificationBot Bot { get; }
        private CancellationTokenSource TokenSource { get; }
        private Thread RunnerThread { get; }

        protected PeriodicAsyncTask(TelegramNotificationBot bot)
        {
            Bot = bot;
            TokenSource = new CancellationTokenSource();
            var token = TokenSource.Token;
            RunnerThread = new Thread(() =>
            {
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        Task.Run(() => TryRunAsync(token), token).Wait(token);
                    }
                    catch (Exception e)
                    {
                        if (!token.IsCancellationRequested)
                            Console.Out.WriteLine(e);
                    }
                }
            });
        }

        public void Run()
        {
            RunnerThread.Start();
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

        private async Task TryRunAsync(CancellationToken token)
        {
            var delay = DefaultDelay;
            var stopwatch = Stopwatch.StartNew();
            try
            {
                await RunAsync().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                delay = DelayAfterError;
                await Bot.NotifyErrorAsync(e).ConfigureAwait(false);
                Console.WriteLine(e);
            }
            stopwatch.Stop();
            if (stopwatch.Elapsed < delay)
                await Task.Delay(delay - stopwatch.Elapsed, token).ConfigureAwait(false);
        }

        protected abstract Task RunAsync();

        protected abstract TimeSpan DefaultDelay { get; }
        protected virtual TimeSpan DelayAfterError => TimeSpan.FromSeconds(30);
    }
}