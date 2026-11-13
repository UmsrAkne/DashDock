using System.Net;
using System.Text.Json;
using DashDock.Models.Memos;
using DashDock.Services.Memos;
using Moq;
using Moq.Protected;

namespace DashDock.Tests.Services.Memos
{
    [TestFixture]
    public class MemosApiClientTests
    {
        [Test]
        public void AttachmentDto_サイズが文字列の場合でも正常にデシリアライズできること()
        {
            // Arrange
            var json = """
            {
                "name": "attachments/gBNRmgZAfJsL7mu75PJrmC",
                "createTime": "2026-11-08T14:59:00Z",
                "filename": "a_694_00001_.png",
                "content": "",
                "externalLink": "",
                "type": "image/png",
                "size": "1920383",
                "memo": "memos/d3qwboa7rwKQWCDvmXjcLT",
                "motionMedia": null
            }
            """;

            // Act
            var attachment = JsonSerializer.Deserialize<AttachmentDto>(json);

            // Assert
            Assert.That(attachment, Is.Not.Null);
            Assert.That(attachment!.Size, Is.EqualTo(1920383L));
            Assert.That(attachment.Filename, Is.EqualTo("a_694_00001_.png"));
            Assert.That(attachment.Name, Is.EqualTo("attachments/gBNRmgZAfJsL7mu75PJrmC"));
            Assert.That(attachment.MimeType, Is.EqualTo("image/png"));
            Assert.That(attachment.IsImage, Is.True);
        }

        [Test]
        public void AttachmentDto_サイズが数値の場合でも正常にデシリアライズできること()
        {
            // Arrange
            var json = """
            {
                "name": "attachments/gBNRmgZAfJsL7mu75PJrmC",
                "createTime": "2026-11-08T14:59:00Z",
                "filename": "a_694_00001_.png",
                "mimeType": "image/png",
                "size": 1920383
            }
            """;

            // Act
            var attachment = JsonSerializer.Deserialize<AttachmentDto>(json);

            // Assert
            Assert.That(attachment, Is.Not.Null);
            Assert.That(attachment!.Size, Is.EqualTo(1920383L));
            Assert.That(attachment.MimeType, Is.EqualTo("image/png"));
            Assert.That(attachment.IsImage, Is.True);
        }

        [Test]
        public async Task GetImageAttachmentsAsync_sizeが文字列のJSONレスポンスを正常に取得できること()
        {
            // Arrange
            var jsonResponse = """
            {
                "attachments": [
                    {
                        "name": "attachments/gBNRmgZAfJsL7mu75PJrmC",
                        "createTime": "2026-11-08T14:59:00Z",
                        "filename": "a_694_00001_.png",
                        "content": "",
                        "externalLink": "",
                        "type": "image/png",
                        "size": "1920383",
                        "memo": "memos/d3qwboa7rwKQWCDvmXjcLT",
                        "motionMedia": null
                    }
                ],
                "nextPageToken": "token123"
            }
            """;

            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse, System.Text.Encoding.UTF8, "application/json"),
                });

            using var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("https://example.com/"),
            };

            var client = new MemosApiClient(httpClient);

            // Act
            var result = await client.GetImageAttachmentsAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Attachments, Has.Count.EqualTo(1));
            Assert.That(result.Attachments[0].Size, Is.EqualTo(1920383L));
            Assert.That(result.Attachments[0].Filename, Is.EqualTo("a_694_00001_.png"));
            Assert.That(result.Attachments[0].MimeType, Is.EqualTo("image/png"));
            Assert.That(result.Attachments[0].IsImage, Is.True);
            Assert.That(result.NextPageToken, Is.EqualTo("token123"));
        }
    }
}
