namespace SignalRStocksBackend.DTOs;

public class TransactionDto
{
    public string Username { get; set; } = String.Empty;
    public string ShareName { get; set; } = String.Empty;
    public int Amount { get; set; }
    public double Price { get; set; }
    public int UnitsInStockNow { get; set; }
    public bool IsUserBuy { get; set; }
}
