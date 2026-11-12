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
            var fileName = attachment.Name.Replace("attachments/", string.Empty) + ".png";
            var baseDirectory = AppContext.BaseDirectory;
            var path = Path.Combine(baseDirectory, fileName);

            await File.WriteAllBytesAsync(path, bytes);

            var jsonPath = Path.ChangeExtension(path, ".json");
            var json = JsonSerializer.Serialize(attachment);
            await File.WriteAllTextAsync(jsonPath, json);
        }
    }
}