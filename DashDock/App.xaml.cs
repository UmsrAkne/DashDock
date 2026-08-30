using System.Windows;
using DashDock.Services;
using DashDock.Utils;
using DashDock.ViewModels;
using DashDock.Views;

namespace DashDock;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        Console.WriteLine("Registering types");

        var settings = AppSettings.Load();
        Console.WriteLine("Loaded settings");
        Console.WriteLine($"Settings hash:{settings.GetHashCode()}");

        // 既存の設定ファイルに新しいプロパティが書き込まれていないケースがあり得るため、同じ内容で保存する。
        // これにより、既存ファイルになかったプロパティが追加される。
        settings.Save();

        containerRegistry.RegisterInstance(settings);

        containerRegistry.Register<IToolAreaViewModel, SettingAreaViewModel>();
        containerRegistry.Register<IToolAreaViewModel, PointAreaViewModel>();
        containerRegistry.Register<IToolAreaViewModel, TimeTrackingAreaViewModel>();
        containerRegistry.Register<IToolAreaViewModel, NoteAreaViewModel>();

        containerRegistry.Register<IPointService, PointService>();

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