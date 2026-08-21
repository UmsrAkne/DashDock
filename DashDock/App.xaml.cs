using System.Windows;
using DashDock.Services;
using DashDock.Utils;
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
        Console.WriteLine("Registering types");

        var settings = AppSettings.Load();
        Console.WriteLine("Loaded settings");
        Console.WriteLine($"Settings hash:{settings.GetHashCode()}");
        containerRegistry.RegisterInstance(settings);

        containerRegistry.Register<IToolAreaViewModel, SettingAreaViewModel>();
        containerRegistry.Register<IToolAreaViewModel, PointAreaViewModel>();
        containerRegistry.Register<IToolAreaViewModel, TimeTrackingAreaViewModel>();
        containerRegistry.Register<IToolAreaViewModel, NoteAreaViewModel>();

        #if DEBUG
        containerRegistry.Register<ITimeTrackingService, MockTimeTrackingService>();
        #else
        containerRegistry.Register<ITimeTrackingService, TimeTrackingService>();
        #endif
    }

    protected override Window CreateShell()
    {
        return Container.Resolve<MainWindow>();
    }
}