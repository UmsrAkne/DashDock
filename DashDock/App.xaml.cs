using System.IO;
using System.Net.Http;
using System.Windows;
using DashDock.Services;
using DashDock.Services.Memos;
using DashDock.Utils;
using DashDock.ViewModels;
using DashDock.Views;
using DotNetEnv;

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

        var baseDir = AppContext.BaseDirectory;

        #if DEBUG
        // 実行ディレクトリ (bin/Debug/...) から親を辿ってソリューションルートの .env を探索
        var currentDir = new DirectoryInfo(baseDir);
        var loaded = false;

        while (currentDir != null)
        {
            var envPath = Path.Combine(currentDir.FullName, ".env");
            if (File.Exists(envPath))
            {
                Env.Load(envPath);
                Console.WriteLine($"Loaded .env from: {envPath}");
                loaded = true;
                break;
            }

            currentDir = currentDir.Parent;
        }

        if (!loaded)
        {
            Console.WriteLine("Warning: .env file not found in parent directories.");
        }
        #else
        // リリース時はアプリ配置フォルダ直下の .env をロード
        var releaseEnvPath = Path.Combine(baseDir, ".env");
        if (File.Exists(releaseEnvPath))
        {
            Env.Load(releaseEnvPath);
        }
        #endif

        // 既存の設定ファイルに新しいプロパティが書き込まれていないケースがあり得るため、同じ内容で保存する。
        // これにより、既存ファイルになかったプロパティが追加される。
        settings.Save();

        containerRegistry.RegisterInstance(settings);

        containerRegistry.Register<IToolAreaViewModel, SettingAreaViewModel>();
        containerRegistry.Register<IToolAreaViewModel, PointAreaViewModel>();
        containerRegistry.Register<IToolAreaViewModel, TimeTrackingAreaViewModel>();
        containerRegistry.Register<IToolAreaViewModel, NoteAreaViewModel>();
        containerRegistry.Register<IToolAreaViewModel, ImageGalleryAreaViewModel>();

        containerRegistry.Register<IMemosApiClient, MemosApiClient>();
        containerRegistry.Register<IPointService, PointService>();

        var credentials = new Credentials
        {
            MemosToken = Environment.GetEnvironmentVariable("MEMOS_TOKEN") ?? string.Empty,
            BaseUrl = Environment.GetEnvironmentVariable("MEMOS_BASE_URL") ?? string.Empty,
        };

        containerRegistry.RegisterInstance(credentials);

        var httpClient = new HttpClient();
        if (Uri.TryCreate(credentials.BaseUrl, UriKind.Absolute, out var baseUri))
        {
            httpClient.BaseAddress = baseUri;
        }

        if (!string.IsNullOrEmpty(credentials.MemosToken))
        {
            httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", credentials.MemosToken);
        }

        containerRegistry.RegisterInstance(httpClient);
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