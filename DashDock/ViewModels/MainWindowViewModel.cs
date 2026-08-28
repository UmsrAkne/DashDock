namespace DashDock.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class MainWindowViewModel : BindableBase
    {
        private string title = "Dash Dock";

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

            ToolAreaViewModels = toolAreaViewModelsList;
        }

        public string Title { get => title; set => SetProperty(ref title, value); }

        public IReadOnlyList<IToolAreaViewModel> ToolAreaViewModels { get; }
    }
}