using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DashDock.Utils
{
    public static class ImageColorAnalyzer
    {
        // --- 抽出時のフィルタリング閾値 ---
        private const float MinSaturationThreshold = 0.20f; // これ以下の彩度は無彩色（グレー）として除外
        private const float MinLightnessThreshold = 0.15f; // 暗すぎるピクセルを除外
        private const float MaxLightnessThreshold = 0.92f; // 白飛びに近いピクセルを除外

        // --- 最終出力色の補正閾値（黒系背景で見栄えを保つための下限設定） ---
        private const float MinOutputLightness = 0.50f; // 暗い背景に沈まないための最低明度
        private const float MinOutputSaturation = 0.55f; // 鮮やかさを際立たせるための最低彩度

        public static Task<Color> CalculateDominantColorFromFileAsync(string filePath)
        {
            return Task.Run(() =>
            {
                if (!File.Exists(filePath))
                {
                    return Colors.LightGray;
                }

                try
                {
                    var thumbnail = new BitmapImage();
                    thumbnail.BeginInit();
                    thumbnail.UriSource = new Uri(filePath);
                    thumbnail.CacheOption = BitmapCacheOption.OnLoad;

                    // 幅64pxに縮小してデコード（メモリとCPUを大幅節約）
                    thumbnail.DecodePixelWidth = 64;
                    thumbnail.EndInit();
                    thumbnail.Freeze();

                    return CalculateDominantColor(thumbnail);
                }
                catch
                {
                    return Colors.LightGray;
                }
            });
        }

        public static Color CalculateDominantColor(BitmapSource bitmapSource)
        {
            if (bitmapSource == null)
            {
                return Colors.LightGray;
            }

            // Bgra32 形式に変換
            var formattedBitmap = new FormatConvertedBitmap();
            formattedBitmap.BeginInit();
            formattedBitmap.Source = bitmapSource;
            formattedBitmap.DestinationFormat = PixelFormats.Bgra32;
            formattedBitmap.EndInit();

            var width = formattedBitmap.PixelWidth;
            var height = formattedBitmap.PixelHeight;
            var stride = width * 4;
            var pixels = new byte[height * stride];

            formattedBitmap.CopyPixels(pixels, stride, 0);

            double weightedRed = 0;
            double weightedGreen = 0;
            double weightedBlue = 0;
            double totalWeight = 0;

            // すべてのピクセルが除外された場合のフォールバック用
            long fallbackRed = 0;
            long fallbackGreen = 0;
            long fallbackBlue = 0;
            long fallbackCount = 0;

            for (var i = 0; i < pixels.Length; i += 4)
            {
                var blue = pixels[i];
                var green = pixels[i + 1];
                var red = pixels[i + 2];
                var alpha = pixels[i + 3];

                // 半透明以下は無視
                if (alpha < 128)
                {
                    continue;
                }

                fallbackRed += red;
                fallbackGreen += green;
                fallbackBlue += blue;
                fallbackCount++;

                RgbToHsl(red, green, blue, out var hue, out var saturation, out var lightness);

                // 彩度の低い色や、極端に暗い/明るいピクセルを除外
                if (saturation >= MinSaturationThreshold
                    && lightness >= MinLightnessThreshold
                    && lightness <= MaxLightnessThreshold)
                {
                    // 彩度が高いピクセルほど重みを大きくして合算（鮮やかな色を優先）
                    double weight = saturation * saturation;
                    weightedRed += red * weight;
                    weightedGreen += green * weight;
                    weightedBlue += blue * weight;
                    totalWeight += weight;
                }
            }

            byte baseR;
            byte baseG;
            byte baseB;

            if (totalWeight > 0)
            {
                baseR = (byte)Math.Clamp((int)(weightedRed / totalWeight), 0, 255);
                baseG = (byte)Math.Clamp((int)(weightedGreen / totalWeight), 0, 255);
                baseB = (byte)Math.Clamp((int)(weightedBlue / totalWeight), 0, 255);
            }
            else if (fallbackCount > 0)
            {
                baseR = (byte)(fallbackRed / fallbackCount);
                baseG = (byte)(fallbackGreen / fallbackCount);
                baseB = (byte)(fallbackBlue / fallbackCount);
            }
            else
            {
                return Colors.LightGray;
            }

            // 抽出した色を黒系背景向けに補正（彩度と明度を底上げ）
            RgbToHsl(baseR, baseG, baseB, out var outHue, out var outSaturation, out var outLightness);

            // 彩度を高めに調整
            outSaturation = Math.Max(outSaturation, MinOutputSaturation);

            // 黒背景で見えにくくならないよう明度を底上げ
            outLightness = Math.Max(outLightness, MinOutputLightness);

            return HslToRgb(outHue, outSaturation, outLightness);
        }

        private static void RgbToHsl(byte r, byte g, byte b, out float h, out float s, out float l)
        {
            var rf = r / 255f;
            var gf = g / 255f;
            var bf = b / 255f;

            var max = Math.Max(rf, Math.Max(gf, bf));
            var min = Math.Min(rf, Math.Min(gf, bf));
            var delta = max - min;

            l = (max + min) / 2f;

            if (delta == 0f)
            {
                h = 0f;
                s = 0f;
            }
            else
            {
                s = l <= 0.5f ? delta / (max + min) : delta / (2f - max - min);

                if (Math.Abs(rf - max) < 0.001)
                {
                    h = ((gf - bf) / delta) + (gf < bf ? 6f : 0f);
                }
                else if (Math.Abs(gf - max) < 0.001)
                {
                    h = ((bf - rf) / delta) + 2f;
                }
                else
                {
                    h = ((rf - gf) / delta) + 4f;
                }

                h /= 6f;
            }
        }

        private static Color HslToRgb(float h, float s, float l)
        {
            float r;
            float g;
            float b;

            if (s == 0f)
            {
                r = g = b = l;
            }
            else
            {
                var q = l < 0.5f ? l * (1f + s) : l + s - (l * s);
                var p = (2f * l) - q;

                r = HueToRgb(p, q, h + (1f / 3f));
                g = HueToRgb(p, q, h);
                b = HueToRgb(p, q, h - (1f / 3f));
            }

            return Color.FromRgb(
                (byte)Math.Clamp((int)(r * 255f), 0, 255),
                (byte)Math.Clamp((int)(g * 255f), 0, 255),
                (byte)Math.Clamp((int)(b * 255f), 0, 255));
        }

        private static float HueToRgb(float p, float q, float t)
        {
            if (t < 0f)
            {
                t += 1f;
            }

            if (t > 1f)
            {
                t -= 1f;
            }

            if (t < 1f / 6f)
            {
                return p + ((q - p) * 6f * t);
            }

            if (t < 1f / 2f)
            {
                return q;
            }

            if (t < 2f / 3f)
            {
                return p + ((q - p) * ((2f / 3f) - t) * 6f);
            }

            return p;
        }
    }
}