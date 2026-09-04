using CommunityToolkit.Mvvm.Input;
using DashDock.Services;
using DashDock.Utils;

namespace DashDock.ViewModels
{
    public class PointAreaViewModel : PeriodicTaskAreaViewModelBase, IToolAreaViewModel
    {
        private readonly IPointService pointService;
        private int points;
        private AsyncRelayCommand? initializeAsyncCommand;
        private AsyncRelayCommand? updatePointCommand;

        public PointAreaViewModel()
        {
            // xaml プレビューを表示するために必要。普通にアプリを起動する場合は実行されない。
            Console.WriteLine("[WARN] PointAreaViewModel() execution!");
            Console.WriteLine(" - 通常、デフォルトコンストラクタは実行されません。DI などで問題が起こっている可能性があります。");
            pointService = new PointService(new AppSettings());
        }

        public PointAreaViewModel(IPointService pointService)
        {
            this.pointService = pointService;
        }

        public override string Header { get; set; } = "Daily Points";

        public int Points { get => points; set => SetProperty(ref points, value); }

        public AsyncRelayCommand InitializeAsyncCommand =>
            initializeAsyncCommand ??= new AsyncRelayCommand(async () =>
            {
                await StartPeriodicTaskAsync();
            });

        public AsyncRelayCommand UpdatePointAsyncCommand =>
            updatePointCommand ??= new AsyncRelayCommand(async () =>
            {
                await FetchAsync(CancellationToken.None);
                await Task.Delay(3000);
            });

        protected override TimeSpan Interval { get; } = TimeSpan.FromMinutes(30);

        protected override async Task FetchAsync(CancellationToken token)
        {
            const int maxRetries = 3;
            const int retryDelaySeconds = 30;

            for (var attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    var point = await pointService.GetLatestBalanceAsync();
                    Points = point;
                    return;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] FetchAsync attempt {attempt} failed: {ex.Message}");
                    
                    if (attempt < maxRetries)
                    {
                        try
                        {
                            await Task.Delay(TimeSpan.FromSeconds(retryDelaySeconds), token);
                        }
                        catch (OperationCanceledException)
                        {
                            return;
                        }
                    }
                    else
                    {
                        Console.WriteLine("[ERROR] FetchAsync max retries reached.");
                    }
                }
            }
        }
    }
}