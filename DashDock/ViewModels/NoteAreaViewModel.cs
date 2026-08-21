using DashDock.Utils;

namespace DashDock.ViewModels
{
    public class NoteAreaViewModel : BindableBase, IToolAreaViewModel
    {
        private string url = string.Empty;

        public NoteAreaViewModel()
        {
            // xaml プレビューを表示するために必要。普通にアプリを起動する場合は実行されない。
            Console.WriteLine("[WARN] NoteAreaViewModel() execution!");
            Console.WriteLine(" - 通常、デフォルトコンストラクタは実行されません。DI などで問題が起こっている可能性があります。");
        }

        public NoteAreaViewModel(AppSettings settings)
        {
            Url = settings.NotePageUrl;
        }

        public string Header { get; set; } = "Note Area";

        public string Url { get => url; set => SetProperty(ref url, value); }
    }
}