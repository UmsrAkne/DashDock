using System.Windows;
using DashDock.Utils;

namespace DashDock.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly AppSettings settings;

        public MainWindow(AppSettings settings)
        {
            InitializeComponent();
            this.settings = settings;

            SourceInitialized += MainWindow_SourceInitialized;
            Closing += MainWindow_Closing;
        }

        // --------------------------------------------------
        // 復元処理
        // --------------------------------------------------
        private void MainWindow_SourceInitialized(object? sender, EventArgs e)
        {
            // 初期値（未設定）でなければ位置とサイズを復元
            if (settings is { WindowTop: not null, WindowLeft: not null, })
            {
                Top = settings.WindowTop.Value;
                Left = settings.WindowLeft.Value;
                Width = settings.WindowWidth;
                Height = settings.WindowHeight;

                // 画面外に飛び出していないかチェック（マルチモニター切断対策）
                EnsureWindowIsVisible();
            }

            // ウィンドウ状態（最大化など）の復元
            // ※ Minimized（最小化）で終了していた場合は Normal にする
            WindowState = settings is { WindowState: WindowState.Maximized, } ? WindowState.Maximized : WindowState.Normal;
        }

        // --------------------------------------------------
        // 保存処理
        // --------------------------------------------------
        private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            // 最大化または最小化されている場合は RestoreBounds から通常時のサイズを取得
            var bounds = WindowState == WindowState.Normal
                ? new Rect(Left, Top, Width, Height)
                : RestoreBounds;

            settings.WindowTop = bounds.Top;
            settings.WindowLeft = bounds.Left;
            settings.WindowWidth = bounds.Width;
            settings.WindowHeight = bounds.Height;

            // 最小化状態で終了した場合は Normal として保存
            settings.WindowState = WindowState == WindowState.Minimized
                ? WindowState.Normal
                : WindowState;

            settings.Save();
        }

        // --------------------------------------------------
        // 画面外飛び出し防止
        // --------------------------------------------------
        private void EnsureWindowIsVisible()
        {
            // 仮想画面全体（すべてのモニターを合わせた領域）
            var virtualLeft = SystemParameters.VirtualScreenLeft;
            var virtualTop = SystemParameters.VirtualScreenTop;
            var virtualWidth = SystemParameters.VirtualScreenWidth;
            var virtualHeight = SystemParameters.VirtualScreenHeight;

            // 完全に画面外に出てしまっている場合はプライマリ画面の中央に配置
            var isOffscreen =
                Left + Width < virtualLeft
                || Left > virtualLeft + virtualWidth
                || Top + Height < virtualTop
                || Top > virtualTop + virtualHeight;

            if (isOffscreen)
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }
        }
    }
}