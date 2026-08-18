namespace DashDock.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class MainWindowViewModel : BindableBase
    {
        private string title = "Wpf Application";

        public string Title { get => title; set => SetProperty(ref title, value); }
    }
}