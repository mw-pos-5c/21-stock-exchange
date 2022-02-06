namespace SignalRStocksBackend.Entities;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = String.Empty;
    public double Cash { get; set; }
}
