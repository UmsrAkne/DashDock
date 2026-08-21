using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DashDock.Utils
{
    public sealed class AppSettings
    {
        public string TimeTrackingServerIpv4 { get; set; } = "127.0.0.1";

        public int TimeTrackingApiPort { get; set; } = 8000;

        public string NotePageUrl { get; set; } = "https://www.google.com";

        [JsonIgnore]
        private static string ConfigPath =>
            Path.Combine(AppContext.BaseDirectory, "app_settings.json");

        public static AppSettings Load()
        {
            // 設定ファイルはサイズが小さいので、同期処理でロードでも問題ないはず。
            if (!File.Exists(ConfigPath))
            {
                var settings = new AppSettings();
                settings.Save();
                return settings;
            }

            var json = File.ReadAllText(ConfigPath);
            return JsonSerializer.Deserialize<AppSettings>(json)
                   ?? new AppSettings();
        }

        public void Save()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
            };

            var json = JsonSerializer.Serialize(this, options);
            File.WriteAllText(ConfigPath, json);
        }

        public async Task SaveAsync()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
            };

            var json = JsonSerializer.Serialize(this, options);
            await File.WriteAllTextAsync(ConfigPath, json);
        }
    }
}