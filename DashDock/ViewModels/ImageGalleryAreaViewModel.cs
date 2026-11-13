using DashDock.Utils;

namespace DashDock.ViewModels
{
    public class ImageGalleryAreaViewModel : PeriodicTaskAreaViewModelBase, IToolAreaViewModel
    {
        private readonly Credentials credentials;

        public ImageGalleryAreaViewModel(Credentials credentials)
        {
            this.credentials = credentials;
        }

        public override string Header { get; set; } = "Image Gallery";

        protected override TimeSpan Interval { get; } = TimeSpan.FromMinutes(60);

        protected override Task FetchAsync(CancellationToken token)
        {
            return Task.CompletedTask;
        }
    }
}