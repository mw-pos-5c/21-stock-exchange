namespace SignalRStocksBackend.DTOs;

public class UserDto
{
    public int Id { get; set; }
    public string Name { get; set; } = String.Empty;
    public double Cash { get; set; }
    public List<DepotDto> Depots { get; set; } = new();
}
