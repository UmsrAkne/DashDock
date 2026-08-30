namespace DashDock.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;

    public class StringToBrushConverter : IValueConverter
    {
        private readonly static SolidColorBrush[] Palette =
        [
            new ((Color)ColorConverter.ConvertFromString("#FF5252")),
            new ((Color)ColorConverter.ConvertFromString("#448AFF")),
            new ((Color)ColorConverter.ConvertFromString("#69F0AE")),
            new ((Color)ColorConverter.ConvertFromString("#FFA726")),
            new ((Color)ColorConverter.ConvertFromString("#E040FB")),
            new ((Color)ColorConverter.ConvertFromString("#18FFFF")),
            new ((Color)ColorConverter.ConvertFromString("#FFD740")),
            new ((Color)ColorConverter.ConvertFromString("#FF4081")),
            new ((Color)ColorConverter.ConvertFromString("#B388FF")),
            new ((Color)ColorConverter.ConvertFromString("#64FFDA")),
            new ((Color)ColorConverter.ConvertFromString("#FF6E40")),
            new ((Color)ColorConverter.ConvertFromString("#40C4FF")),
            new ((Color)ColorConverter.ConvertFromString("#EEFF41")),
            new ((Color)ColorConverter.ConvertFromString("#BCAAA4")),
            new ((Color)ColorConverter.ConvertFromString("#90CAF9")),
            new ((Color)ColorConverter.ConvertFromString("#F48FB1")),
        ];

        static StringToBrushConverter()
        {
            // パフォーマンス向上のため Brush をフリーズ
            foreach (var brush in Palette)
            {
                if (brush.CanFreeze)
                {
                    brush.Freeze();
                }
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not string text || string.IsNullOrEmpty(text))
            {
                return Brushes.White; // デフォルト色
            }

            // 簡易ハッシュ計算 (負の数対策で uint を使用)
            var hash = 2166136261;
            foreach (var c in text)
            {
                hash = (hash ^ c) * 16777619;
            }

            var index = (int)(hash % (uint)Palette.Length);
            return Palette[index];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}