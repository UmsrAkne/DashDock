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
            return Task.FromResult(new TimeEntry());
        }
    }
}