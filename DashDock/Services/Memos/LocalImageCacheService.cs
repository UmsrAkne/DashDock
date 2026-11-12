using System.IO;
using System.Text.Json;
using DashDock.Models.Memos;

namespace DashDock.Services.Memos
{
    public class LocalImageCacheService
    {
        /// <summary>
        /// 入力された AttachmentDto からファイル名を生成してローカルにファイルとして byte 配列を保存します。
        /// 同時に attachment の情報をサイドカーファイルとして保存します。
        /// </summary>
        /// <param name="attachment">生成するファイルの情報</param>
        /// <param name="bytes">生成するファイルのバイト配列</param>
        public async Task SaveLocalImageAsync(AttachmentDto attachment, byte[] bytes)
        {
            // "name": "attachments/gBNRmgZAfJsL7mu75PJrmC",
            var fileName = attachment.GetAttachmentId() + ".png";
            var path = BuildFilePath(fileName);

            await File.WriteAllBytesAsync(path, bytes);

            var jsonPath = Path.ChangeExtension(path, ".json");
            var json = JsonSerializer.Serialize(attachment);
            await File.WriteAllTextAsync(jsonPath, json);
        }

        /// <summary>
        /// attachment の情報をもとに、ローカルにファイルが存在するか判定します。
        /// </summary>
        /// <param name="attachment">チェックする画像のメタデータ。</param>
        /// <returns>該当する画像がローカルに存在するかどうか。</returns>
        public bool ExistsLocalImage(AttachmentDto attachment)
        {
            var filePath = BuildFilePath(attachment.GetAttachmentId() + ".png");
            return File.Exists(filePath);
        }

        private string BuildFilePath(string fileName)
        {
            var baseDirectory = AppContext.BaseDirectory;
            return Path.Combine(baseDirectory, fileName);
        }
    }
}