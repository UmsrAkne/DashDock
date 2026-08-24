using DashDock.Utils;

namespace DashDock.Services
{
    public class PointService : IPointService
    {
        public PointService(AppSettings settings)
        {
            Console.WriteLine("PointService created");
            Console.WriteLine($"Settings hash:{settings.GetHashCode()}");
        }
    }
}