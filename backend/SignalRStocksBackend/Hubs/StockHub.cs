using Microsoft.AspNetCore.SignalR;

using SignalRStocksBackend.DTOs;
using SignalRStocksBackend.Services;

namespace SignalRStocksBackend.Hubs;

public class StockHub : Hub<IStockHub>
{
    private readonly StockService stockService;

    private static int clientCount = 0;

    public StockHub(StockService stockService)
    {
        this.stockService = stockService;
    }

    public override Task OnConnectedAsync()
    {
        clientCount++;
        Clients.All.ClientCountChanged(clientCount);
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        clientCount--;
        Clients.All.ClientCountChanged(clientCount);
        return base.OnDisconnectedAsync(exception);
    }

    public void BuyShare(TransactionDto transactionDto)
    {
        Clients.All.TransactionReceived(transactionDto);
    }

    public void SellShare(TransactionDto transactionDto)
    {
        Clients.All.TransactionReceived(transactionDto);
    }
}
