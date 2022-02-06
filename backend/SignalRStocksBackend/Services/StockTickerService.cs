using MathNet.Numerics.Interpolation;
using Microsoft.AspNetCore.SignalR;
using SignalRStocksBackend.DTOs;
using SignalRStocksBackend.Entities;
using SignalRStocksBackend.Hubs;

// ** comment in when the Hub is ready
// using SignalRStocksBackend.Hubs;

namespace SignalRStocksBackend.Services;

public class StockTickerService
{
    private readonly Random random = new();
    // ** comment in when the Hub is ready
    private readonly StockHub stockHub;  
    private readonly StockContext db;
    private readonly Dictionary<string, List<Tuple<double, double>>> stockData = new();
    private readonly Dictionary<string, CubicSpline> splines = new();
    

    public int NrSplinePoints { get; set; } = 1000;
    public int MinInterpolationPoints { get; set; } = 20;
    public int MaxInterpolationPoints { get; set; } = 40;
    public double MaxChangePercent { get; set; } = 5;
    public double MaxWhiteNoisePercent { get; set; } = 1;

    public int TickSpeed { get; set; } = 1;

    // ** comment in when the Hub is ready
    public StockTickerService(StockHub stockHub, StockContext db)
    {
        // ** comment in when the Hub is ready
        this.stockHub = stockHub;
        this.db = db;
        PrepareStockData();
        PrintComparison("StockA");
        Task.Run(() => StartStockTicker());
    }

    private void PrintComparison(string name)
    {
        Console.WriteLine("StockTickerService::PrintComparison for {name}");
        var data = stockData[name];
        var spline = splines[name];
        foreach (var item in data)
        {
            double x = item.Item1;
            double yReal = item.Item2;
            double y = spline.Interpolate(x);
            Console.WriteLine($"{x:0.0}/{name}: {y:0.0} / {yReal:0.0}");
        }
    }

    private void PrepareStockData()
    {
        Console.WriteLine("StockTickerService::PrepareStockData");
        var names = db.Shares.Select(x => x.Name).ToList();

        InitializeStocksWithStartValue(db.Shares.OrderBy(x => x.Name).ToList());
        PrepareTimelineForStocks(names);
        PrepareSplines(names);
    }

    private void InitializeStocksWithStartValue(List<Share> shares)
    {
        Console.WriteLine($"StockTickerService::InitializeStocksWithStartValue for {shares.Count} stocks");
        foreach (var share in shares)
        {
            stockData[share.Name] = new List<Tuple<double, double>>();
            //double startValue = random.NextDouble() * 100 + 100; //range [100,199]
            double startValue = share.StartPrice * (random.NextDouble() + 0.5); //range Course on 30.12. +/-50%
            stockData[share.Name].Add(new Tuple<double, double>(0, startValue));
        }
    }

    private void PrepareTimelineForStocks(List<string> names)
    {
        Console.WriteLine($"StockTickerService::PrepareTimelineForStocks for {names.Count} stocks");
        int nrValues = 100;
        for (int i = 0; i < nrValues; i++)
        {
            foreach (var name in names)
            {
                var points = stockData[name];
                double x = points.Last().Item1;
                double y = points.Last().Item2;
                int stepX = random.Next(MinInterpolationPoints, MaxInterpolationPoints);
                double changePercent = (random.NextDouble() * 2 - 1) * MaxChangePercent;
                double delta = y * changePercent / 100;
                x += stepX;
                y += delta;
                if (y < 1) y = 1;
                points.Add(new Tuple<double, double>(x, y));
            }
        }
    }

    private void PrepareSplines(List<string> names)
    {
        Console.WriteLine("StockTickerService::PrepareSplines");
        foreach (var name in names)
        {
            var xValues = stockData[name].Select(x => x.Item1).ToArray();
            var yValues = stockData[name].Select(x => x.Item2).ToArray();
            splines[name] = CubicSpline.InterpolateAkimaSorted(xValues, yValues);
        }
    }

    private async void StartStockTicker()
    {
        double x = 0;
        double step = 1;
        double maxNoisePerc = MaxWhiteNoisePercent / 100;
        while (true)
        {
            Console.WriteLine($"StockTickerService::StockTicker update stock exchange prices");
            var stocks = new List<ShareTickDto>();
            foreach (var name in splines.Keys.OrderBy(x => x))
            {
                var spline = splines[name];
                double y = spline.Interpolate(x);
                //var item = stockData[name].FirstOrDefault(p => p.Item1 == x);
                //if (item != null) Console.WriteLine($"   {x:0.0}/{name}: {y:0.00} (real value)");
                double noise = y * maxNoisePerc * (random.NextDouble() * 2 - 1);
                y += noise;
                if (y < 0.5) y = 0.5;
                //Console.WriteLine($"   {x:0.0}/{name}: {y:0.00}");
                stocks.Add(new ShareTickDto
                {
                    Name = name,
                    Val = y
                });
            }
            Console.WriteLine($"StockService::SendNewStocks via Hub: {stocks.Count} stocks");
            // ** comment in when the Hub is ready
            
            if (stockHub.Clients != null)
            {
                await stockHub.Clients.All.NewStockData(stocks);
            }
            x += step;
            int delay = TickSpeed > 0 ? 2000 / TickSpeed : 2000;
            await Task.Delay(delay);
        }

    }

}
