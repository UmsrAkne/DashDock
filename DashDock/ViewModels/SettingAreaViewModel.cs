using MaterialDesignThemes.Wpf;

namespace DashDock.ViewModels
{
    public class SettingAreaViewModel : BindableBase, IToolAreaViewModel
    {
        public string Header { get; set; } = "Setting Area";

        public bool Visible { get; set; }

        public PackIconKind IconKind { get; set; } = PackIconKind.Settings;
    }
}