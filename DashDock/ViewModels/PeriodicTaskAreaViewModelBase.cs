namespace DashDock.ViewModels
{
    public abstract class PeriodicTaskAreaViewModelBase : BindableBase, IToolAreaViewModel, IDisposable
    {
        private readonly CancellationTokenSource cts = new();
        private bool disposed;
        private bool visible;

        public abstract string Header { get; set; }

        // Todo: 別の課題で このクラスから IToolAreaViewModel を切り離す。
        // 理由: このクラスが IToolAreaViewModel を実装しているのは設計的に不自然であるため。
        public bool Visible { get => visible; set => SetProperty(ref visible, value); }

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