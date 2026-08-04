using DataAccess.Models;
using DataAccess.Repositories;
using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public class PortfoliosNotificationsService : IPortfoliosNotificationsService
    {
        private readonly IPortfoliosNotificationsRepository _portfoliosNotificationsRepository;
        private readonly ITransactionsService _transactionsService;
        private readonly IUsersRepository _usersRepository;
        private readonly IEmailService _emailService;

        public PortfoliosNotificationsService(IPortfoliosNotificationsRepository portfoliosNotificationsRepository, ITransactionsService transactionsService, IUsersRepository usersRepository, IEmailService emailService)
        {
            _portfoliosNotificationsRepository = portfoliosNotificationsRepository;
            _transactionsService = transactionsService;
            _usersRepository = usersRepository;
            _emailService = emailService;
        }

        public PortfolioNotification? FindByPortfolioId(int portfolioId)
        {
            IzvestuvanjaPortfolija? notification = _portfoliosNotificationsRepository.GetByPortfolioId(portfolioId);

            if (notification == null)
            {
                return new PortfolioNotification
                {
                    PortfolioId = portfolioId,
                    IsEnabled = false,
                    Frequency = "Weekly"
                };
            }

            return new PortfolioNotification
            {
                PortfolioId = notification.PortfolioId,
                IsEnabled = notification.Ovozmozeno,
                Frequency = notification.Frekvencija
            };
        }

        public PortfolioNotification Update(UpdatePortfolioNotification portfolioNotification)
        {
            if (portfolioNotification.Frequency != "Weekly" && portfolioNotification.Frequency != "Monthly")
                throw new ArgumentException("Frequency must be Weekly or Monthly.");

            IzvestuvanjaPortfolija? foundNotification = _portfoliosNotificationsRepository.GetByPortfolioId(portfolioNotification.PortfolioId);

            if (foundNotification == null)
            {
                foundNotification = new IzvestuvanjaPortfolija
                {
                    PortfolioId = portfolioNotification.PortfolioId,
                    Ovozmozeno = portfolioNotification.IsEnabled,
                    Frekvencija = portfolioNotification.Frequency,
                    PoslednoIsprateno = null
                };

                _portfoliosNotificationsRepository.Add(foundNotification);
            }
            else
            {
                foundNotification.Ovozmozeno = portfolioNotification.IsEnabled;
                foundNotification.Frekvencija = portfolioNotification.Frequency;

                IzvestuvanjaPortfolija updated = _portfoliosNotificationsRepository.Update(foundNotification);
            }

            return new PortfolioNotification
            {
                PortfolioId = portfolioNotification.PortfolioId,
                IsEnabled = foundNotification.Ovozmozeno,
                Frequency = foundNotification.Frekvencija
            };
        }

        public void SendScheduledNotifications()
        {
            IEnumerable<Korisnici> users = _usersRepository.GetAll();

            foreach(Korisnici user in users)
            {
                IEnumerable<IzvestuvanjaPortfolija?> notifications = _portfoliosNotificationsRepository.GetByUserId(user.Id);

                IEnumerable<IzvestuvanjaPortfolija?> enabledNotifications = notifications
                    .Where(n => n.Ovozmozeno && ShouldSend(n))
                    .ToList();
                if (!enabledNotifications.Any())
                    continue;

                foreach(IzvestuvanjaPortfolija notification in  enabledNotifications)
                {
                    PortfolioAnalytics realAnalytics = _transactionsService.GetAnalytics(notification.PortfolioId, true);
                    PortfolioAnalytics simulatedAnalytics = _transactionsService.GetAnalytics(notification.PortfolioId, false);

                    string body = BuildEmailBody(notification.PortfolioId, realAnalytics, simulatedAnalytics);

                    _emailService.SendEmail(user.Email, "Your Portfolio Performance Summary", body);

                    notification.PoslednoIsprateno = DateTime.UtcNow;
                    _portfoliosNotificationsRepository.Update(notification);
                }
            }
        }

        private bool ShouldSend(IzvestuvanjaPortfolija notification)
        {
            if (!notification.PoslednoIsprateno.HasValue) return true;

            return notification.Frekvencija switch
            {
                "Weekly" => notification.PoslednoIsprateno.Value.AddDays(7) <= DateTime.UtcNow,
                "Monthly" => notification.PoslednoIsprateno.Value.AddMonths(1) <= DateTime.UtcNow,
                _ => false
            };
        }

        private string BuildEmailBody(int portfolioId, PortfolioAnalytics real, PortfolioAnalytics simulated)
        {
            return $@"
            <h3>Portfolio Performance Summary</h3>
            <p><strong>Portfolio Id:</strong> {portfolioId}</p>
            <h4>Real Transactions</h4>
            <p>Total Revenue: {real.TotalRevenue:C}</p>
            <p>Total Expenses: {real.TotalExpenses:C}</p>
            <p>Balance: {real.Balance:C}</p>

            <h4>Simulated Transactions</h4>
            <p>Total Revenue: {simulated.TotalRevenue:C}</p>
            <p>Total Expenses: {simulated.TotalExpenses:C}</p>
            <p>Balance: {simulated.Balance:C}</p>
            ";
        }
    }
}
