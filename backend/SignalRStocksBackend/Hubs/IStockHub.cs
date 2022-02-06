using SignalRStocksBackend.DTOs;

namespace SignalRStocksBackend.Hubs;

public interface IStockHub
{
    public Task NewStockData(List<ShareTickDto> shareTickDtos);
    public Task ClientCountChanged(int count);
    public Task TransactionReceived(TransactionDto transaction);
}
