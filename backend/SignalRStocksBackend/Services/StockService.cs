using SignalRStocksBackend.DTOs;
using SignalRStocksBackend.Entities;

namespace SignalRStocksBackend.Services;

public class StockService
{
    private readonly StockContext db;

    public StockService(StockContext db)
    {
        this.db = db;
    }

    public IEnumerable<ShareDto> GetAllShares()
    {
        return db.Shares.Select(s => new ShareDto
        {
            Id = s.Id,
            Name = s.Name,
            UnitsInStock = s.UnitsInStock
        });
    }

    public IEnumerable<DepotDto> GetUserDepots(string name)
    {
        return db.UserShares.Where(share => share.User.Name.Equals(name)).Select(share => new DepotDto
        {
            ShareName = share.Share.Name,
            Amount = share.Amount
        });
    }

    public double GetUserCash(string name)
    {
        return db.Users.FirstOrDefault(user => user.Name.Equals(name))?.Cash ?? 0;
    }

    public TransactionDto? AddTransaction(string username, string shareName, int amount, bool isBuy)
    {
        User? user = db.Users.FirstOrDefault(user => user.Name.Equals(username));
        Share? share = db.Shares.FirstOrDefault(share => share.Name.Equals(shareName));

        if (user == null || share == null) return null;

        double shareCost = share.Price * amount;
        UserShare? userShare = db.UserShares.FirstOrDefault(userShare => userShare.User.Equals(user) && userShare.Share.Equals(share));

        switch (isBuy)
        {
            case true when user.Cash < shareCost:
                return null;
            case true:
            {
                if (userShare == null)
                {
                    db.UserShares.Add(new UserShare
                    {
                        Amount = amount,
                        Share = share,
                        User = user
                    });
                }
                else
                {
                    userShare.Amount += amount;
                }
                user.Cash -= shareCost;
                db.SaveChanges();

                break;
            }
            case false:
            {
                if (userShare == null)
                {
                    return null;
                }

                if (amount > userShare.Amount)
                {
                    return null;
                }

                userShare.Amount -= amount;
                user.Cash += shareCost;
                db.SaveChanges();

                break;
            }
        }

        TransactionDto transaction = new TransactionDto
        {
            Amount = amount,
            ShareName = share.Name,
            Username = user.Name,
            IsUserBuy = isBuy,
            Price = shareCost
        };

        return transaction;
    }
}
