using CommunityToolkit.Mvvm.Input;
using DashDock.Models;
using DashDock.Services;

namespace DashDock.ViewModels
{
    public sealed class TimeTrackingAreaViewModel : PeriodicTaskAreaViewModelBase
    {
        private readonly ITimeTrackingService service;
        private TimeEntry? currentEntry;
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

        public override string Header { get; set; } = "Time Tracking Area";

        public TimeEntry? CurrentEntry
        {
            get => currentEntry;
            set => SetProperty(ref currentEntry, value);
        }

        public AsyncRelayCommand InitializeAsyncCommand =>
            initializeAsyncCommand ??= new AsyncRelayCommand(async () =>
            {
                await StartPeriodicTaskAsync();
            });

        protected override TimeSpan Interval { get; } = TimeSpan.FromMinutes(2);

        protected override async Task FetchAsync(CancellationToken token)
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
    }
}