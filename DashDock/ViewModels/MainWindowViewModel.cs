namespace DashDock.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class MainWindowViewModel : BindableBase
    {
        private string title = "Dash Dock";
        private bool isSettingsOpen;

        // Xaml プレビューで使用されるコンストラクタ。ユーザーが明示的に呼び出すことはない。
        public MainWindowViewModel()
        {
            Console.WriteLine("Execute: MainWindowViewModel()");

            // プレビューを表示するためにビューモデルを詰め込む。
            ToolAreaViewModels = new List<IToolAreaViewModel>()
            {
                new PointAreaViewModel(),
                new TimeTrackingAreaViewModel(),
                new NoteAreaViewModel(),
            };
        }

        public MainWindowViewModel(IEnumerable<IToolAreaViewModel> toolAreaViewModels)
        {
            Console.WriteLine("Execute: MainWindowViewModel(toolAreaViewModels)");
            var toolAreaViewModelsList = toolAreaViewModels.ToList();
            foreach (var toolAreaViewModel in toolAreaViewModelsList)
            {
                Console.WriteLine(toolAreaViewModel);
            }

            var settingAreaViewModel = toolAreaViewModelsList.FirstOrDefault(x => x is SettingAreaViewModel);
            if (settingAreaViewModel != null)
            {
                SettingAreaViewModel = (SettingAreaViewModel)settingAreaViewModel;
            }

            ToolAreaViewModels = toolAreaViewModelsList.Where(x => x != settingAreaViewModel).ToList();
        }

        public string Title { get => title; set => SetProperty(ref title, value); }

        public IReadOnlyList<IToolAreaViewModel> ToolAreaViewModels { get; }

        public SettingAreaViewModel SettingAreaViewModel { get; set; } = new ();
        
        public bool IsSettingsOpen
        {
            get => isSettingsOpen;
            set => SetProperty(ref isSettingsOpen, value);
        }

        public DelegateCommand OpenSettingsCommand => new DelegateCommand(() =>
        {
            IsSettingsOpen = !IsSettingsOpen;
        });
    }
}