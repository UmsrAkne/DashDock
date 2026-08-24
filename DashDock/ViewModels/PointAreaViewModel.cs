using DashDock.Services;
using DashDock.Utils;

namespace DashDock.ViewModels
{
    public class PointAreaViewModel : BindableBase, IToolAreaViewModel
    {
        private int points;
        private readonly IPointService pointService;

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

        public string Header { get; set; } = "Point Area";

        public int Points { get => points; set => SetProperty(ref points, value); }
    }
}