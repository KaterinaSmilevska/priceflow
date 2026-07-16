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

        public async Task GenerateNotificationsAsync(DateTime tradingDate)
        {
            DateTime date = tradingDate.Date;

            List<DnevenPromet> dailyTurnover = await _dbContext.DnevenPromet
                .Where(d => d.Datum >= date && d.Datum < date.AddDays(1) && d.ProcentPromena != null)
                .ToListAsync();

            foreach(DnevenPromet dp in dailyTurnover)
            {
                List<HvPromenaCena> alerts = await _dbContext.HvPromenaCena
                    .Include(a => a.Hv)
                    .Where(a => a.Hvid == dp.Hvid)
                    .ToListAsync();

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
                        

                    bool exists = await _dbContext.IzvestuvanjaPromenaCena
                        .AnyAsync(n =>
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
                        await _dbContext.IzvestuvanjaPromenaCena.AddAsync(notification);
                    }
                }
            }
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IzvestuvanjaPromenaCena?> GetById(int id)
        {
            return await _dbContext.IzvestuvanjaPromenaCena.FindAsync(id);
        }

        public async Task<List<IzvestuvanjaPromenaCena>> GetByUserAsync(int userId)
        {
            return await _dbContext.IzvestuvanjaPromenaCena
                .Where(n => n.KorisnikId == userId)
                .OrderByDescending(n => n.DatumTrguvanje)
                .ToListAsync();
        }

        public async Task<int> GetUnreadNotificationCountAsync(int userId)
        {
            return await _dbContext.IzvestuvanjaPromenaCena
                .CountAsync(n => n.KorisnikId == userId && n.Procitano == false);
        }

        public async Task MarkNotificationAsReadAsync(int notificationId)
        {
            IzvestuvanjaPromenaCena? notification = await GetById(notificationId);
            if(notification != null)
            {
                notification.Procitano = true;
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<bool> NotificationExistsForDateAsync(DateTime date)
        {
            return await _dbContext.IzvestuvanjaPromenaCena
                .AnyAsync(n => n.DatumTrguvanje >= date.Date && n.DatumTrguvanje < date.Date.AddDays(1));
        }
    }
}
