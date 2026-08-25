namespace DashDock.Services
{
    public interface IPointService
    {
        Task<int> GetLatestBalanceAsync();
    }
}