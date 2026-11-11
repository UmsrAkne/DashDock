using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using DashDock.Models.Memos;

namespace DashDock.Services.Memos
{
    public class MemosApiClient : IMemosApiClient
    {
        private readonly HttpClient httpClient;
        private readonly JsonSerializerOptions jsonOptions;

        public MemosApiClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
            jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                NumberHandling = JsonNumberHandling.AllowReadingFromString,
            };
        }

        /// <summary>
        /// 画像アタッチメント一覧を取得します。
        /// </summary>
        public async Task<ListAttachmentsResponse> GetImageAttachmentsAsync(
            int pageSize = 50,
            string? pageToken = null,
            CancellationToken cancellationToken = default)
        {
            // memos API の CEL フィルタを使用して画像形式のみに絞り込み
            var filterQuery = Uri.EscapeDataString("mime_type.startsWith('image/')");
            var requestUrl = $"api/v1/attachments?pageSize={pageSize}&filter={filterQuery}";

            if (!string.IsNullOrEmpty(pageToken))
            {
                requestUrl += $"&pageToken={Uri.EscapeDataString(pageToken)}";
            }

            using var response = await httpClient.GetAsync(requestUrl, cancellationToken);
            response.EnsureSuccessStatusCode();

            await using var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
            var result = await JsonSerializer.DeserializeAsync<ListAttachmentsResponse>(
                contentStream,
                jsonOptions,
                cancellationToken);

            if (result == null)
            {
                return new ListAttachmentsResponse();
            }

            return result;
        }

        /// <summary>
        /// アタッチメントの画像バイナリをバイト配列としてダウンロードします。
        /// </summary>
        public async Task<byte[]> DownloadAttachmentBytesAsync(
            AttachmentDto attachment,
            CancellationToken cancellationToken = default)
        {
            if (attachment == null)
            {
                throw new ArgumentNullException(nameof(attachment));
            }

            var downloadUrl = BuildDownloadUrl(attachment);
            using var response = await httpClient.GetAsync(downloadUrl, cancellationToken);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsByteArrayAsync(cancellationToken);
        }

        /// <summary>
        /// アタッチメントのダウンロードURLを構築します。
        /// </summary>
        private string BuildDownloadUrl(AttachmentDto attachment)
        {
            // S3 などの外部ストレージ直リンクが存在する場合は優先
            if (!string.IsNullOrEmpty(attachment.ExternalLink))
            {
                return attachment.ExternalLink;
            }

            // memos の標準ファイル配信パス: /file/{attachment.name}/{attachment.filename}
            // attachment.name は "attachments/123" 形式
            if (!string.IsNullOrEmpty(attachment.Filename))
            {
                return $"file/{attachment.Name}/{Uri.EscapeDataString(attachment.Filename)}";
            }

            // ファイル名がない場合のフォールバック（/file/{attachment.name}）
            return $"file/{attachment.Name}";
        }
    }
}