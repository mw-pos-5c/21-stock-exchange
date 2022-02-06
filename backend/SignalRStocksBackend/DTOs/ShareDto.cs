namespace SignalRStocksBackend.DTOs;

public class ShareDto
{
    public int Id { get; set; }
    public string Name { get; set; } = String.Empty;
    public int UnitsInStock { get; set; }
}
