using System.Net.Http;
using System.Net.Http.Json;
using DashDock.Models;
using DashDock.Utils;

namespace DashDock.Services
{
    public class TimeTrackingService : ITimeTrackingService
    {
        private const string ApiEndPoint = "/api/elite-tasks/active-task";
        private readonly static HttpClient HttpClient = new();
        private readonly string serverIpv4Address;
        private readonly int port;

        public TimeTrackingService(AppSettings settings)
        {
            Console.WriteLine("TimeTrackingService created");
            Console.WriteLine($"Settings hash:{settings.GetHashCode()}");
            serverIpv4Address = settings.TimeTrackingServerIpv4;
            port = settings.TimeTrackingApiPort;
        }

        public Task PostTimeEntryAsync(TimeEntry timeEntry)
        {
            return Task.CompletedTask;
        }

        public async Task<TimeEntry> GetCurrentTimeEntryAsync()
        {
            try
            {
                var url = $"http://{serverIpv4Address}:{port}{ApiEndPoint}";
                var response = await HttpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var eliteTask = await response.Content.ReadFromJsonAsync<TimeEntryResponse>();
                if (eliteTask == null)
                {
                    return new TimeEntry();
                }

                return new TimeEntry
                {
                    StartTime = DateTimeOffset.FromUnixTimeSeconds(eliteTask.T1).LocalDateTime,
                    EndTime = DateTimeOffset.FromUnixTimeSeconds(eliteTask.T2).LocalDateTime,
                    Description = eliteTask.Ds ?? string.Empty,
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching active task: {ex.Message}");
                return new TimeEntry();
            }
        }
    }
}