namespace DashDock.ViewModels
{
    public class PointAreaViewModel : BindableBase, IToolAreaViewModel
    {
        private int points;

        public string Header { get; set; } = "Point Area";

        public int Points { get => points; set => SetProperty(ref points, value); }
    }
}