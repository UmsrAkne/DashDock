using DashDock.Models.Memos;

namespace DashDock.Services.Memos
{
    public interface IMemosApiClient
    {
        /// <summary>
        /// 画像アタッチメントの一覧を取得します。
        /// </summary>
        /// <param name="pageSize">1回のリクエストで取得する件数（最大1000）</param>
        /// <param name="pageToken">次ページ取得用トークン</param>
        /// <param name="cancellationToken">キャンセレーショントークン</param>
        /// <returns>アタッチメント一覧と次ページトークンのレスポンス</returns>
        Task<ListAttachmentsResponse> GetImageAttachmentsAsync(
            int pageSize = 50,
            string? pageToken = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// アタッチメントの画像バイナリデータを取得します。
        /// </summary>
        /// <param name="attachment">取得対象のアタッチメント情報</param>
        /// <param name="cancellationToken">キャンセレーショントークン</param>
        /// <returns>画像バイト配列</returns>
        Task<byte[]> DownloadAttachmentBytesAsync(
            AttachmentDto attachment,
            CancellationToken cancellationToken = default);
    }
}