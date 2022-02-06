namespace SignalRStocksBackend.DTOs;

public class ShareTickDto
{
    public string Name { get; set; } = String.Empty;
    public double Val { get; set; }
    public override string ToString() => $"{Name} {Val:0.0}";
}
