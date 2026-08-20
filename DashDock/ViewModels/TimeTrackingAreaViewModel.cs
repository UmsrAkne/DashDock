using DashDock.Services;

namespace DashDock.ViewModels
{
    public class TimeTrackingAreaViewModel : BindableBase, IToolAreaViewModel
    {
        private readonly ITimeTrackingService service;

        public TimeTrackingAreaViewModel(ITimeTrackingService service)
        {
            Console.WriteLine("TimeTrackingAreaViewModel created");
            this.service = service;

            Console.WriteLine($"injected service: {service}");
        }

        public string Header { get; set; } = "Time Tracking Area";
    }
}