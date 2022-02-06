namespace SignalRStocksBackend.Entities;

public class Share
{
    public int Id { get; set; }
    public string Name { get; set; } = String.Empty;
    public int UnitsInStock { get; set; }
    public double StartPrice { get; set; }
}
