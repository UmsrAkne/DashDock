using System.Windows.Threading;
using CommunityToolkit.Mvvm.Input;
using DashDock.Models;
using DashDock.Services;

namespace DashDock.ViewModels
{
    public sealed class TimeTrackingAreaViewModel : PeriodicTaskAreaViewModelBase
    {
        private readonly ITimeTrackingService service;
        private readonly DispatcherTimer dispatcherTimer = new ();
        private readonly string defaultTitle = "Dash Dock";
        private TimeEntry? currentEntry;
        private AsyncRelayCommand? initializeAsyncCommand;
        private TimeSpan elapsedTimeSpan;

        // 更新までの残り時間のインジケーター用のフィールド
        private TimeSpan remainingTimeToRefresh;
        private DateTime nextFetchTime = DateTime.MinValue;
        private double refreshMaxSeconds = 120.0;
        private double refreshProgress;

        private bool disposed;
        private string header = string.Empty;

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

            Header = defaultTitle;

            dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
            dispatcherTimer.Tick += OnTick;
            dispatcherTimer.Start();
        }

        public override string Header { get => header; set => SetProperty(ref header, value); }

        public TimeEntry? CurrentEntry
        {
            get => currentEntry;
            set => SetProperty(ref currentEntry, value);
        }

        public TimeSpan ElapsedTimeSpan
        {
            get => elapsedTimeSpan;
            set => SetProperty(ref elapsedTimeSpan, value);
        }

        public AsyncRelayCommand InitializeAsyncCommand =>
            initializeAsyncCommand ??= new AsyncRelayCommand(async () =>
            {
                await StartPeriodicTaskAsync();
            });

        public TimeSpan RemainingTimeToRefresh
        {
            get => remainingTimeToRefresh;
            private set => SetProperty(ref remainingTimeToRefresh, value);
        }

        public double RefreshMaxSeconds
        {
            get => refreshMaxSeconds;
            set => SetProperty(ref refreshMaxSeconds, value);
        }

        public double RefreshProgress
        {
            get => refreshProgress;
            set => SetProperty(ref refreshProgress, value);
        }

        protected override TimeSpan Interval { get; } = TimeSpan.FromMinutes(2);

        protected override async Task FetchAsync(CancellationToken token)
        {
            try
            {
                nextFetchTime = DateTime.Now + Interval;
                var entry = await service.GetCurrentTimeEntryAsync();
                CurrentEntry = entry;
            }
            catch (Exception ex)
            {
                // 通信エラーなどのハンドリング
                Console.WriteLine(ex);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // このクラスでのみ行う終了処理
                    dispatcherTimer.Stop();
                    dispatcherTimer.Tick -= OnTick;
                }

                disposed = true;
            }

            base.Dispose(disposing);
        }

        private void OnTick(object? sender, EventArgs e)
        {
            if (CurrentEntry is not null)
            {
                var entry = CurrentEntry;
                var elapsed = DateTime.Now - entry.StartTime;
                ElapsedTimeSpan = elapsed;
                Header = $"{(int)ElapsedTimeSpan.TotalMinutes}m | {CurrentEntry.Description}";
            }

            var t = nextFetchTime - DateTime.Now;
            if (t > TimeSpan.Zero)
            {
                RemainingTimeToRefresh = t;
                RefreshProgress = t.TotalSeconds;
            }
            else
            {
                RemainingTimeToRefresh = TimeSpan.Zero;
            }
        }
    }
}