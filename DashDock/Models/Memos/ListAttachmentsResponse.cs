using System.Text.Json.Serialization;

namespace DashDock.Models.Memos
{
    /// <summary>
    /// アタッチメント一覧取得 API (GET /api/attachments) のレスポンス
    /// </summary>
    public record ListAttachmentsResponse
    {
        [JsonPropertyName("attachments")]
        public IReadOnlyList<AttachmentDto> Attachments { get; init; } = Array.Empty<AttachmentDto>();

        [JsonPropertyName("nextPageToken")]
        public string? NextPageToken { get; init; }
    }
}