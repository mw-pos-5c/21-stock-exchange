using SignalRStocksBackend.DTOs;
using SignalRStocksBackend.Entities;

namespace SignalRStocksBackend.Services;

public class StockService
{
    private readonly StockContext db;

    public StockService(StockContext db)
    {
        this.db = db;
    }

}
