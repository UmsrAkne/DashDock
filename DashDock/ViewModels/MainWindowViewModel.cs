namespace DashDock.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class MainWindowViewModel : BindableBase
    {
        private string title = "Wpf Application";

        public MainWindowViewModel()
        {
            Console.WriteLine("Execute: MainWindowViewModel()");
        }

        public MainWindowViewModel(IEnumerable<IToolAreaViewModel> toolAreaViewModels)
        {
            Console.WriteLine("Execute: MainWindowViewModel(toolAreaViewModels)");
            var toolAreaViewModelsList = toolAreaViewModels.ToList();
            foreach (var toolAreaViewModel in toolAreaViewModelsList)
            {
                Console.WriteLine(toolAreaViewModel);
            }
        }

        public string Title { get => title; set => SetProperty(ref title, value); }
    }
}