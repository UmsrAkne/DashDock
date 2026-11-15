using CommunityToolkit.Mvvm.Input;
using DashDock.Services.Memos;
using MaterialDesignThemes.Wpf;

namespace DashDock.ViewModels
{
    public class ImageGalleryAreaViewModel : PeriodicTaskAreaViewModelBase, IToolAreaViewModel
    {
        private readonly IMemosApiClient? memosApiClient;
        private readonly LocalImageCacheService? localImageCacheService;
        private string? imagePath;
        private AsyncRelayCommand? initializeAsyncCommand;
        private bool visible = true;

        public ImageGalleryAreaViewModel()
        {
            // XAML プレビュー用
        }

        public ImageGalleryAreaViewModel(IMemosApiClient memosApiClient, LocalImageCacheService localImageCacheService)
        {
            this.memosApiClient = memosApiClient;
            this.localImageCacheService = localImageCacheService;
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
                var response = await memosApiClient.GetImageAttachmentsAsync(cancellationToken: token);
                var attachment = response?.Attachments[new Random().Next(0, response.Attachments.Count - 1)];
                if (attachment == null)
                {
                    return;
                }

                if (!localImageCacheService.ExistsLocalImage(attachment))
                {
                    var bytes = await memosApiClient.DownloadAttachmentBytesAsync(attachment, token);
                    await localImageCacheService.SaveLocalImageAsync(attachment, bytes);
                }

                ImagePath = localImageCacheService.GetLocalImagePath(attachment);
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
    }
}