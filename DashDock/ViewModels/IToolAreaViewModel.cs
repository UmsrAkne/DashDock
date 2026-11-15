using MaterialDesignThemes.Wpf;

namespace DashDock.ViewModels
{
    public interface IToolAreaViewModel
    {
        public string Header { get; set; }

        public bool Visible { get; set; }

        public PackIconKind IconKind { get; set; }

        public bool CanExpand { get; set; }
    }
}