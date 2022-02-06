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

    public IEnumerable<ShareDto> GetAllShares()
    {
        return db.Shares.Select(s => new ShareDto
        {
            Id = s.Id,
            Name = s.Name,
            UnitsInStock = s.UnitsInStock
        });
    }
}
