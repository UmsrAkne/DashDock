namespace DashDock.ViewModels
{
    public abstract class PeriodicTaskAreaViewModelBase : BindableBase, IToolAreaViewModel, IDisposable
    {
        private readonly CancellationTokenSource cts = new();
        private bool disposed;

        public abstract string Header { get; set; }

        protected abstract TimeSpan Interval { get; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected async Task StartPeriodicTaskAsync()
        {
            using var timer = new PeriodicTimer(Interval);
            try
            {
                await FetchAsync(cts.Token); // 初回
                while (await timer.WaitForNextTickAsync(cts.Token))
                {
                    await FetchAsync(cts.Token); // 定期実行
                }
            }
            catch (OperationCanceledException)
            {
                // 正常停止
            }
        }

        protected abstract Task FetchAsync(CancellationToken token);

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                cts.Cancel();
                cts.Dispose();
            }

            disposed = true;
        }
    }
}