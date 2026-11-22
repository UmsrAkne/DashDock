using CommunityToolkit.Mvvm.Input;

namespace DashDock.ViewModels
{
    public abstract class PeriodicTaskAreaViewModelBase : BindableBase, IDisposable
    {
        private readonly CancellationTokenSource cts = new ();
        private readonly SemaphoreSlim fetchLock = new (1, 1);
        private CancellationTokenSource? timerResetCts;
        private AsyncRelayCommand? manualFetchCommand;
        private bool disposed;

        /// <summary>
        ///     ユーザー操作から手動実行するためのコマンド
        /// </summary>
        public AsyncRelayCommand ManualFetchCommand =>
            manualFetchCommand ??= new AsyncRelayCommand(async () =>
            {
                await ExecuteManualFetchAsync();
            });

        protected abstract TimeSpan Interval { get; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     手動で Fetch を実行し、インターバルの進捗をリセットします。
        /// </summary>
        public async Task ExecuteManualFetchAsync()
        {
            if (disposed || cts.IsCancellationRequested)
            {
                return;
            }

            // 現在の待機をキャンセルしてタイマーをリセット
            timerResetCts?.Cancel();

            // 即時実行
            await ExecuteFetchInternalAsync(cts.Token);
        }

        protected async Task StartPeriodicTaskAsync()
        {
            try
            {
                // 初回実行
                await ExecuteFetchInternalAsync(cts.Token);

                while (!cts.IsCancellationRequested)
                {
                    // 手動実行によってキャンセル（待機リセット）可能なトークンを作成
                    timerResetCts?.Dispose();
                    timerResetCts = CancellationTokenSource.CreateLinkedTokenSource(cts.Token);

                    try
                    {
                        // 次の実行まで待機
                        await Task.Delay(Interval, timerResetCts.Token);
                    }
                    catch (OperationCanceledException) when (!cts.IsCancellationRequested)
                    {
                        // 手動実行によるリセットが発生した場合は待機ループの最初からやり直す
                        continue;
                    }

                    // 定期実行
                    await ExecuteFetchInternalAsync(cts.Token);
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
                timerResetCts?.Cancel();
                timerResetCts?.Dispose();
                fetchLock.Dispose();
                cts.Dispose();
            }

            disposed = true;
        }

        private async Task ExecuteFetchInternalAsync(CancellationToken token)
        {
            // 定期実行と手動実行の同時実行を防止
            await fetchLock.WaitAsync(token);
            try
            {
                await FetchAsync(token);
            }
            finally
            {
                fetchLock.Release();
            }
        }
    }
}