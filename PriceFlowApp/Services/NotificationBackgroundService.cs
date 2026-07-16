namespace PriceFlowApp.Services
{
    public class NotificationBackgroundService: BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public NotificationBackgroundService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IPortfoliosNotificationsService>();

                await service.SendScheduledNotificationsAsync();

                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }
    }
}
