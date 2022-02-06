namespace SignalRStocksBackend.Entities;

public class StockContext
{
    public StockContext()
    {
        CreateShares();
        CreateUsers();
    }

    public List<User> Users { get; set; } = new();
    public List<Share> Shares { get; set; } = new();
    public List<UserShare> UserShares { get; set; } = new();
    public List<Transaction> Transactions { get; set; } = new();

    private readonly Dictionary<string, (int, double)> coursesAndStock = new()
    {
        ["Andritz"] = (37, 99),
        ["AT&S"] = (26, 2.8),
        ["BawagGroup"] = (38, 89),
        ["CAImmo"] = (31, 98),
        ["DO&CO"] = (68, 9.74),
        ["Erste_Group"] = (25, 378.47),
        ["Immofinanz"] = (17, 112.09),
        ["Lenzing"] = (82, 26.55),
        ["MayrMelnhof"] = (166, 20),
        ["OMV"] = (34, 326.98),
        ["ÖsterreichischePost"] = (29, 67.55),
        ["Raiffeisen"] = (17, 328.62),
        ["SchoellerBeckmann"] = (32, 15.96),
        ["SparkassenImmo"] = (17, 72.26),
        ["TelekomAustria"] = (6, 4.74),
        ["Uniqua"] = (6, 306.97),
        ["Verbund"] = (69, 170.23),
        ["ViennaInsurance"] = (21, 128),
        ["Voestalpine"] = (30, 178.55),
        ["Wienerberger"] = (26, 112.27),
    };

    private void CreateShares()
    {
        var random = new Random();
        int id = 1;
        coursesAndStock.Keys.ToList().ForEach(x => Shares.Add(new Share
        {
            Id = id++,
            Name = x,
            StartPrice = coursesAndStock[x].Item1 * (random.NextDouble() + 0.5), //range Course on 30.12. +/-50%
            UnitsInStock = (int)(coursesAndStock[x].Item2 * 1000) //are usually millions, set to thousands here
        }));
    }

    private void CreateUsers()
    {
        var names = new List<string> { "Hansi", "Pepi", "Susi", "Charly", "Fritzi" };
        int id = 1;
        names.ForEach(x => Users.Add(new User
        {
            Id = id++,
            Name = x,
            Cash = 200000,
        }));
    }

    internal void SaveChanges()
    {
        //ignored here
    }
}

