namespace DashDock.Services
{
    public interface IPointService
    {
        Task<int> GetLatestBalanceAsync(string ipv4, int port);
    }
}