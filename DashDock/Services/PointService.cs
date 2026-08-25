using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using DashDock.Utils;

namespace DashDock.Services
{
    public sealed class PointService : IPointService, IDisposable
    {
        private readonly HttpClient httpClient = new ();
        private bool disposed;

        public PointService(AppSettings settings)
        {
            Console.WriteLine("PointService created");
            Console.WriteLine($"Settings hash:{settings.GetHashCode()}");
        }

        public async Task<int> GetLatestBalanceAsync(string ipv4, int port)
        {
            var url = $"http://{ipv4}:{port}/api/transactions?limit=2";
            var response = await httpClient.GetFromJsonAsync<TransactionResponse>(url);

            if (response?.Transactions == null || response.Transactions.Count == 0)
            {
                throw new InvalidOperationException("No transactions found.");
            }

            // 最新の balance を取り出して返す
            return response.Transactions[0].Balance;
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
        }
    }
}