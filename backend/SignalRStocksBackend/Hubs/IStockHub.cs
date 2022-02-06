using SignalRStocksBackend.DTOs;

namespace SignalRStocksBackend.Hubs;

public interface IStockHub
{
    public Task NewStockData(List<ShareTickDto> shareTickDtos);
}
