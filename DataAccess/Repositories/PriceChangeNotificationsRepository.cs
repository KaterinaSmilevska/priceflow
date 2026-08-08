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

        public IEnumerable<IzvestuvanjaPromenaCena> GetByUserId(int userId)
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

        public void MarkNotificationAsRead(IzvestuvanjaPromenaCena notification)
        {
            notification.Procitano = true;
            _dbContext.SaveChanges();
        }

        public bool ExistsForUserAndSecurityAndDate(int userId, int securityId, DateTime date)
        {
            return _dbContext.IzvestuvanjaPromenaCena
                .Any(n =>
                n.KorisnikId == userId &&
                n.Hvid == securityId &&
                n.DatumTrguvanje >= date &&
                n.DatumTrguvanje < date.AddDays(1));
        }

        public IzvestuvanjaPromenaCena Add(IzvestuvanjaPromenaCena notification)
        {
            _dbContext.IzvestuvanjaPromenaCena.Add(notification);
            _dbContext.SaveChanges();

            return notification;
        }
    }
}
