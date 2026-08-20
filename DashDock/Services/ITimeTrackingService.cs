using DashDock.Models;

namespace DashDock.Services
{
    public interface ITimeTrackingService
    {
        Task PostTimeEntryAsync(TimeEntry timeEntry);

        Task<TimeEntry> GetCurrentTimeEntryAsync();
    }
}