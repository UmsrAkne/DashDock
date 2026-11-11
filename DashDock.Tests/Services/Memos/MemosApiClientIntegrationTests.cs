using DashDock.Models.Memos;
using DashDock.Services.Memos;
using DashDock.Utils;
using DotNetEnv;

namespace DashDock.Tests.Services.Memos
{
    [TestFixture]
    public class MemosApiClientIntegrationTests
    {
        // 検証用の一時的な接続情報（※コミット時は注意するか環境変数等から取得）
        private string baseUrl = "https://your-memos-host.example.com";
        private string personalAccessToken = "your_actual_access_token_here";

        private HttpClient httpClient;
        private MemosApiClient client;

        [SetUp]
        public void SetUp()
        {
            Env.TraversePath().Load();

            var credentials = new Credentials
            {
                MemosToken = Environment.GetEnvironmentVariable("MEMOS_TOKEN") ?? string.Empty,
                BaseUrl = Environment.GetEnvironmentVariable("MEMOS_BASE_URL") ?? string.Empty,
            };

            Console.WriteLine(credentials.BaseUrl);

            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(credentials.BaseUrl);
            httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", credentials.MemosToken);

            client = new MemosApiClient(httpClient);
        }

        [TearDown]
        public void TearDown()
        {
            httpClient.Dispose();
        }

        [Test]
        [Explicit("実際のサーバーに通信するため、手動確認時のみ実行する")]
        public async Task GetResourcesAsync_実際にAPIを叩いてリソース一覧が取得できること()
        {
            // Act
            var resources = await client.GetImageAttachmentsAsync();

            // Assert
            Assert.That(resources, Is.Not.Null);
            Console.WriteLine($"取得件数: {resources.Attachments.Count}");

            foreach (var resource in resources.Attachments)
            {
                Console.WriteLine($"Filename: {resource.Filename}");
            }
        }

        [Test]
        [Explicit("実際のサーバーに通信するため、手動確認時のみ実行する")]
        public async Task DownloadResourceAsync_実際に画像バイナリをダウンロードできること()
        {
            // 存在するリソース名/IDを指定
            var resourceName = "resources/xxxx";

            // Act
            var bytes = await client.DownloadAttachmentBytesAsync(new AttachmentDto());

            // Assert
            Assert.That(bytes, Is.Not.Null);
            Assert.That(bytes.Length, Is.GreaterThan(0));

            Console.WriteLine($"ダウンロードサイズ: {bytes.Length} bytes");
        }
    }
}