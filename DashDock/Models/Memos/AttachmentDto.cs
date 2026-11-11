using System.Text.Json.Serialization;

namespace DashDock.Models.Memos
{
    /// <summary>
    /// memos のアタッチメント (Resource) 情報
    /// </summary>
    public record AttachmentDto
    {
        /// <summary>
        /// リソース識別名 (例: "attachments/123" または "attachments/xyz")
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; init; } = string.Empty;

        /// <summary>
        /// ファイル名 (例: "sample.png")
        /// </summary>
        [JsonPropertyName("filename")]
        public string Filename { get; init; } = string.Empty;

        private string? mimeType;

        /// <summary>
        /// MIMEタイプ (例: "image/png", "image/jpeg") - memos API の type フィールド
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; init; } = string.Empty;

        /// <summary>
        /// MIMEタイプ (例: "image/png", "image/jpeg")
        /// </summary>
        [JsonPropertyName("mimeType")]
        public string MimeType
        {
            get => !string.IsNullOrEmpty(mimeType) ? mimeType : Type;
            init => mimeType = value;
        }

        /// <summary>
        /// ファイルサイズ (バイト数)
        /// </summary>
        [JsonPropertyName("size")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public long Size { get; init; }

        /// <summary>
        /// 作成日時
        /// </summary>
        [JsonPropertyName("createTime")]
        public DateTimeOffset? CreateTime { get; init; }

        /// <summary>
        /// 外部ストレージリンク（S3等を使用している場合）
        /// </summary>
        [JsonPropertyName("externalLink")]
        public string? ExternalLink { get; init; }

        /// <summary>
        /// 画像ファイルかどうかを判定するヘルパープロパティ
        /// </summary>
        [JsonIgnore]
        public bool IsImage
        {
            get
            {
                if (string.IsNullOrEmpty(MimeType))
                {
                    return false;
                }

                return MimeType.StartsWith("image/", StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}