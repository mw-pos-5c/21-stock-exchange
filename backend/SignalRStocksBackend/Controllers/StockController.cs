using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SignalRStocksBackend.DTOs;
using SignalRStocksBackend.Services;

namespace SignalRStocksBackend.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class StockController : ControllerBase
{
    private readonly StockService stockService;

    public StockController(StockService stockService)
    {
  
        this.stockService = stockService;
    }


    [HttpGet]
    public string Testerl()
    {
        Console.WriteLine($"StockController::Testerl");
        return "Done";
    }
    
    [HttpGet]
    public IEnumerable<ShareDto> GetShares()
    {
        return stockService.GetAllShares();
    }
}
