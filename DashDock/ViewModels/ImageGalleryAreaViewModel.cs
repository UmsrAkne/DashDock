using DashDock.Utils;

namespace DashDock.ViewModels
{
    public class ImageGalleryAreaViewModel : IToolAreaViewModel
    {
        private readonly Credentials credentials;

        public ImageGalleryAreaViewModel(Credentials credentials)
        {
            this.credentials = credentials;
        }

        public string Header { get; set; } = "Image Gallery";
    }
}