using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories
{
    public class PriceChangeNotificationsRepository : IPriceChangeNotificationsRepository
    {
        private readonly PriceFlowDbContext _dbContext;

        public PriceChangeNotificationsRepository(PriceFlowDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IzvestuvanjaPromenaCena? GetById(int id)
        {
            return  _dbContext.IzvestuvanjaPromenaCena
                .Find(id);
        }

        public IEnumerable<IzvestuvanjaPromenaCena?> GetByUserId(int userId)
        {
            return _dbContext.IzvestuvanjaPromenaCena
                .Where(n => n.KorisnikId == userId)
                .OrderByDescending(n => n.DatumTrguvanje)
                .ToList();
        }

        public int GetUnreadNotificationCount(int userId)
        {
            return _dbContext.IzvestuvanjaPromenaCena
                .Count(n => n.KorisnikId == userId && n.Procitano == false);
        }

        public void MarkNotificationAsRead(int notificationId)
        {
            IzvestuvanjaPromenaCena? notification = GetById(notificationId);
            if (notification != null)
            {
                notification.Procitano = true;
                _dbContext.SaveChanges();
            }
        }

        public void GenerateNotifications(DateTime tradingDate)
        {
            DateTime date = tradingDate.Date;

            IEnumerable<DnevenPromet> dailyTurnover = _dbContext.DnevenPromet
                .Where(d => d.Datum >= date && d.Datum < date.AddDays(1) && d.ProcentPromena != null)
                .ToList();

            foreach(DnevenPromet dp in dailyTurnover)
            {
                IEnumerable<HvPromenaCena> alerts = _dbContext.HvPromenaCena
                    .Include(a => a.Hv)
                    .Where(a => a.Hvid == dp.Hvid)
                    .ToList();

                foreach(HvPromenaCena alert in alerts)
                {
                    int netQuantity = _dbContext.Transakcii
                        .Join(_dbContext.Portfolija, t => t.PortfolioId, p => p.Id, (t, p) => new { t, p })
                        .Where(x => x.p.KorisnikId == alert.KorisnikId && x.t.Hvid == alert.Hvid)
                        .AsEnumerable()
                        .Sum(x => x.t.TipTransakcija == "Купување" ? x.t.KolicinaAkcii :
                            x.t.TipTransakcija == "Продавање" ? -x.t.KolicinaAkcii : 0);

                    if (netQuantity <= 0)
                    {
                        continue;
                    }
                        

                    bool exists = _dbContext.IzvestuvanjaPromenaCena
                        .Any(n =>
                            n.KorisnikId == alert.KorisnikId &&
                            n.Hvid == alert.Hvid &&
                            n.DatumTrguvanje >= date && n.DatumTrguvanje < date.AddDays(1)
                        );

                    if(!exists && (dp.ProcentPromena <= alert.DolnaGranica || dp.ProcentPromena >= alert.GornaGranica))
                    {
                        IzvestuvanjaPromenaCena notification = new IzvestuvanjaPromenaCena
                        {
                            KorisnikId = alert.KorisnikId,
                            Hvid = alert.Hvid,
                            ProcentPromena = dp.ProcentPromena.Value,
                            DatumTrguvanje = tradingDate,
                            Poraka =
                                $"Security {alert.Hv.Kod} changed {dp.ProcentPromena:F2}% " +
                                $"(Threshold: {alert.DolnaGranica}% / {alert.GornaGranica}%)",
                            Procitano = false
                        };
                        _dbContext.IzvestuvanjaPromenaCena.Add(notification);
                    }
                }
            }
            _dbContext.SaveChanges();
        }

        public bool NotificationExistsForDate(DateTime date)
        {
            return _dbContext.IzvestuvanjaPromenaCena
                .Any(n => n.DatumTrguvanje >= date.Date && n.DatumTrguvanje < date.Date.AddDays(1));
        }
    }
}
