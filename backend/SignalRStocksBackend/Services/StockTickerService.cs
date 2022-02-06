#region usings

using MathNet.Numerics.Interpolation;

using Microsoft.AspNetCore.SignalR;

using SignalRStocksBackend.DTOs;
using SignalRStocksBackend.Entities;
using SignalRStocksBackend.Hubs;

#endregion

// ** comment in when the Hub is ready
// using SignalRStocksBackend.Hubs;

namespace SignalRStocksBackend.Services;

public class StockTickerService : IHostedService
{
    #region Constants and Fields

    private readonly StockContext db;
    private readonly Random random = new();
    private readonly Dictionary<string, CubicSpline> splines = new();

    private readonly Dictionary<string, List<Tuple<double, double>>> stockData = new();

    // ** comment in when the Hub is ready

    private readonly IHubContext<StockHub> stockHub;

    #endregion

    public StockTickerService(IHubContext<StockHub> stockHub, StockContext db)
    {
        this.stockHub = stockHub;
        this.db = db;
    }

    public double MaxChangePercent { get; set; } = 5;
    public int MaxInterpolationPoints { get; set; } = 40;
    public double MaxWhiteNoisePercent { get; set; } = 1;
    public int MinInterpolationPoints { get; set; } = 20;

    public int NrSplinePoints { get; set; } = 1000;

    public int TickSpeed { get; set; } = 1;

    private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

    public Task StartAsync(CancellationToken cancellationToken)
    {
        PrepareStockData();

        Task.Run(() => StartStockTicker(cancellationTokenSource.Token), cancellationToken);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        cancellationTokenSource.Cancel();
        return Task.CompletedTask;
    }

    private void InitializeStocksWithStartValue(List<Share> shares)
    {
        Console.WriteLine($"StockTickerService::InitializeStocksWithStartValue for {shares.Count} stocks");
        foreach (Share share in shares)
        {
            stockData[share.Name] = new List<Tuple<double, double>>();

            //double startValue = random.NextDouble() * 100 + 100; //range [100,199]
            double startValue = share.StartPrice * (random.NextDouble() + 0.5); //range Course on 30.12. +/-50%
            stockData[share.Name].Add(new Tuple<double, double>(0, startValue));
        }
    }

    private void PrepareSplines(List<string> names)
    {
        Console.WriteLine("StockTickerService::PrepareSplines");
        foreach (string name in names)
        {
            double[] xValues = stockData[name].Select(x => x.Item1).ToArray();
            double[] yValues = stockData[name].Select(x => x.Item2).ToArray();
            splines[name] = CubicSpline.InterpolateAkimaSorted(xValues, yValues);
        }
    }

    private void PrepareStockData()
    {
        Console.WriteLine("StockTickerService::PrepareStockData");
        List<string> names = db.Shares.Select(x => x.Name).ToList();

        InitializeStocksWithStartValue(db.Shares.OrderBy(x => x.Name).ToList());
        PrepareTimelineForStocks(names);
        PrepareSplines(names);
    }

    private void PrepareTimelineForStocks(List<string> names)
    {
        Console.WriteLine($"StockTickerService::PrepareTimelineForStocks for {names.Count} stocks");
        var nrValues = 100;
        for (var i = 0; i < nrValues; i++)
        {
            foreach (string name in names)
            {
                List<Tuple<double, double>> points = stockData[name];
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

    private void PrintComparison(string name)
    {
        Console.WriteLine($"StockTickerService::PrintComparison for {name}");
        List<Tuple<double, double>> data = stockData[name];
        CubicSpline spline = splines[name];
        foreach (Tuple<double, double> item in data)
        {
            double x = item.Item1;
            double yReal = item.Item2;
            double y = spline.Interpolate(x);
            Console.WriteLine($"{x:0.0}/{name}: {y:0.0} / {yReal:0.0}");
        }
    }

    private async Task StartStockTicker(CancellationToken cancellationToken)
    {
        double x = 0;
        double step = 1;
        double maxNoisePerc = MaxWhiteNoisePercent / 100;
        while (!cancellationToken.IsCancellationRequested)
        {
            Console.WriteLine($"StockTickerService::StockTicker update stock exchange prices");
            var stocks = new List<ShareTickDto>();
            foreach (string name in splines.Keys.OrderBy(x => x))
            {
                CubicSpline spline = splines[name];
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
                await stockHub.Clients.All.SendAsync("NewStockData", stocks);
            }

            x += step;
            int delay = TickSpeed > 0
                ? 2000 / TickSpeed
                : 2000;
            await Task.Delay(delay, cancellationToken);
            
        }

        Console.WriteLine("Exiting graceful!");
    }
}
