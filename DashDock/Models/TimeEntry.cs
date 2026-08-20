namespace DashDock.Models
{
    public class TimeEntry
    {
        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string Description { get; set; } = string.Empty;
    }
}