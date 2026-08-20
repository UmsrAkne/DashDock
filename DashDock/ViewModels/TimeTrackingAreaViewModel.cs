using CommunityToolkit.Mvvm.Input;
using DashDock.Models;
using DashDock.Services;

namespace DashDock.ViewModels
{
    public sealed class TimeTrackingAreaViewModel : BindableBase, IToolAreaViewModel, IDisposable
    {
        private readonly ITimeTrackingService service;
        private readonly CancellationTokenSource cts = new();
        private TimeEntry? currentEntry;
        private bool disposed;
        private AsyncRelayCommand? initializeAsyncCommand;

        public TimeTrackingAreaViewModel()
        {
            // xaml プレビューを表示するために必要。普通にアプリを起動する場合は実行されない。
            Console.WriteLine("[WARN] TimeTrackingAreaViewModel() execution!");
            Console.WriteLine(" - 通常、デフォルトコンストラクタは実行されません。DI などで問題が起こっている可能性があります。");
            service = new MockTimeTrackingService();
            CurrentEntry = service.GetCurrentTimeEntryAsync().Result;
        }

        public TimeTrackingAreaViewModel(ITimeTrackingService service)
        {
            Console.WriteLine("TimeTrackingAreaViewModel created");
            this.service = service;

            Console.WriteLine($"injected service: {service}");
        }

        public string Header { get; set; } = "Time Tracking Area";

        public TimeEntry? CurrentEntry
        {
            get => currentEntry;
            set => SetProperty(ref currentEntry, value);
        }

        public AsyncRelayCommand InitializeAsyncCommand =>
            initializeAsyncCommand ??= new AsyncRelayCommand(async () =>
            {
                await StartTimerLoopAsync(cts.Token);
            });

        public void Dispose()
        {
            Dispose(true);
        }

        private async Task StartTimerLoopAsync(CancellationToken token)
        {
            using var timer = new PeriodicTimer(TimeSpan.FromMinutes(2));
            try
            {
                await FetchAsync(); // 初回実行

                while (await timer.WaitForNextTickAsync(token))
                {
                    await FetchAsync(); // 定期実行
                }
            }
            catch (OperationCanceledException)
            {
                 /* 正常停止 */
            }
        }

        private async Task FetchAsync()
        {
            try
            {
                var entry = await service.GetCurrentTimeEntryAsync();
                CurrentEntry = entry;
            }
            catch (Exception ex)
            {
                // 通信エラーなどのハンドリング
                Console.WriteLine(ex);
            }
        }

        private void Dispose(bool disposing)
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