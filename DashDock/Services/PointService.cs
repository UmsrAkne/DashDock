using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using DashDock.Utils;

namespace DashDock.Services
{
    public sealed class PointService : IPointService, IDisposable
    {
        private readonly AppSettings settings;
        private readonly HttpClient httpClient = new ();
        private bool disposed;

        public PointService(AppSettings settings)
        {
            Console.WriteLine("PointService created");
            Console.WriteLine($"Settings hash:{settings.GetHashCode()}");
            this.settings = settings;
        }

        public async Task<int> GetLatestBalanceAsync()
        {
            var url = $"http://{settings.PointServerIpv4}:{settings.PointApiPort}/api/transactions?limit=20";
            const int maxRetries = 3;

            for (var attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    var response = await httpClient.GetFromJsonAsync<TransactionResponse>(url);

                    if (response?.Transactions == null || response.Transactions.Count == 0)
                    {
                        // データが0件なのは「通信成功したけど中身がない」状態なので、リトライしても無駄。即座に例外を投げる
                        throw new InvalidOperationException("No transactions found.");
                    }

                    // 最新のレコードを検索するにはシーケンスナンバーの方を見る。サーバー側の仕様により、日時では確実な順番が保証されない
                    return response.Transactions.OrderByDescending(t => t.SequenceNumber).First().Balance;
                }
                catch (HttpRequestException ex) when (attempt < maxRetries)
                {
                    // 通信エラー時のみリトライ（InvalidOperationException などはリトライせずに抜ける）
                    // 簡易的な待機（例: 1回目失敗で500ms、2回目失敗で1000ms待つ）
                    await Task.Delay(attempt * 500);
                }
            }

            throw new InvalidOperationException("ここには到達しないはずですが、コンパイラ警告防止用");
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                httpClient.Dispose();
            }

            disposed = true;
        }

        private class TransactionResponse
        {
            [JsonPropertyName("transactions")]
            public List<Transaction> Transactions { get; init; } = new ();
        }

        private class Transaction
        {
            [JsonPropertyName("balance")]
            public int Balance { get; set; }

            [JsonPropertyName("sequence_number")]
            public int SequenceNumber { get; set; }
        }
    }
}