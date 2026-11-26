using System.Windows.Media;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.Input;
using DashDock.Services.Memos;
using DashDock.Utils;
using MaterialDesignThemes.Wpf;

namespace DashDock.ViewModels
{
    public class ImageGalleryAreaViewModel : PeriodicTaskAreaViewModelBase, IToolAreaViewModel
    {
        private readonly IMemosApiClient? memosApiClient;
        private readonly LocalImageCacheService? localImageCacheService;
        private readonly DispatcherTimer dispatcherTimer;
        private string? imagePath;
        private AsyncRelayCommand? initializeAsyncCommand;
        private bool visible = true;
        private DateTime nextFetchTime;
        private TimeSpan remainingTimeToRefresh;
        private double refreshProgress;
        private SolidColorBrush progressBarBrush = new (Colors.LightSkyBlue);

        public ImageGalleryAreaViewModel()
        {
            // XAML プレビュー用
            dispatcherTimer = new DispatcherTimer();
        }

        public ImageGalleryAreaViewModel(IMemosApiClient memosApiClient, LocalImageCacheService localImageCacheService)
        {
            this.memosApiClient = memosApiClient;
            this.localImageCacheService = localImageCacheService;
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = TimeSpan.FromMinutes(1);
            dispatcherTimer.Tick += OnTick;
            dispatcherTimer.Start();
            nextFetchTime = DateTime.Now + TimeSpan.FromSeconds(RefreshMaxSeconds);
            UpdateRemainingTime();
        }

        public string Header { get; set; } = "Image Gallery";

        public bool Visible { get => visible; set => SetProperty(ref visible, value); }

        public PackIconKind IconKind { get; set; } = PackIconKind.Image;

        public bool CanExpand { get; set; } = true;

        public string? ImagePath
        {
            get => imagePath;
            set => SetProperty(ref imagePath, value);
        }

        public double RefreshProgress
        {
            get => refreshProgress;
            set => SetProperty(ref refreshProgress, value);
        }

        public TimeSpan RemainingTimeToRefresh
        {
            get => remainingTimeToRefresh;
            set => SetProperty(ref remainingTimeToRefresh, value);
        }

        public double RefreshMaxSeconds { get; } = 3600;

        public SolidColorBrush ProgressBarBrush
        {
            get => progressBarBrush;
            set => SetProperty(ref progressBarBrush, value);
        }

        public AsyncRelayCommand InitializeAsyncCommand =>
            initializeAsyncCommand ??= new AsyncRelayCommand(async () =>
            {
                await StartPeriodicTaskAsync();
            });

        protected override TimeSpan Interval { get; } = TimeSpan.FromHours(1);

        protected override async Task FetchAsync(CancellationToken token)
        {
            if (memosApiClient == null || localImageCacheService == null)
            {
                return;
            }

            try
            {
                nextFetchTime = DateTime.Now.AddSeconds(RefreshMaxSeconds);
                var response = await memosApiClient.GetImageAttachmentsAsync(cancellationToken: token);
                var attachment = response?.Attachments[new Random().Next(0, response.Attachments.Count)];
                if (attachment == null)
                {
                    return;
                }

                if (!localImageCacheService.ExistsLocalImage(attachment))
                {
                    var bytes = await memosApiClient.DownloadAttachmentBytesAsync(attachment, token);
                    await localImageCacheService.SaveLocalImageAsync(attachment, bytes);
                }

                var localPath = localImageCacheService.GetLocalImagePath(attachment);
                ImagePath = localPath;

                var dominantColor = await ImageColorAnalyzer.CalculateDominantColorFromFileAsync(localPath);
                var brush = new SolidColorBrush(dominantColor);
                brush.Freeze();
                ProgressBarBrush = brush;
            }
            catch (OperationCanceledException)
            {
                // 正常停止
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] ImageGalleryAreaViewModel.FetchAsync failed: {ex.Message}");
            }
        }

        private void OnTick(object? sender, EventArgs e)
        {
            UpdateRemainingTime();
        }

        private void UpdateRemainingTime()
        {
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