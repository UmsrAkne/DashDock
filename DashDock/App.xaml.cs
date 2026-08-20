using System.Windows;
using DashDock.ViewModels;
using DashDock.Views;

namespace DashDock;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : PrismApplication
{
    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.Register<IToolAreaViewModel, PointAreaViewModel>();
        containerRegistry.Register<IToolAreaViewModel, TimeTrackingAreaViewModel>();
        containerRegistry.Register<IToolAreaViewModel, NoteAreaViewModel>();
    }

    protected override Window CreateShell()
    {
        return Container.Resolve<MainWindow>();
    }
}