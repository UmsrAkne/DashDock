using DashDock.Models;

namespace DashDock.Services
{
    public class MockTimeTrackingService : ITimeTrackingService
    {
        public Task PostTimeEntryAsync(TimeEntry timeEntry)
        {
            return Task.CompletedTask;
        }

        public Task<TimeEntry> GetCurrentTimeEntryAsync()
        {
            Console.WriteLine("GetCurrentTimeEntryAsync called");

            const int timeOffset = 10;
            var te = new TimeEntry()
            {
                StartTime = DateTime.Now.AddMinutes(-timeOffset),
                EndTime = DateTime.Now.AddMinutes(-timeOffset),
                Description = $"Mock Time Entry ({timeOffset}分前に開始)",
            };

            return Task.FromResult(te);
        }
    }
}