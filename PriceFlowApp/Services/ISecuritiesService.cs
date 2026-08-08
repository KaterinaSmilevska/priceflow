using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public interface ISecuritiesService
    {
        Security FindById(int id);

        Security FindByCode(string code);

        IEnumerable<Security> FindAll();

        string? FindSecurityCode(int id);

        int? FindTotalNumShares(int id);

        int? FindTotalNumShares(string securityCode);

        Security Add(AddSecurityRequest security);

        Security Update(int id, UpdateSecurity security);

        Security Delete(int id);

        SecurityDailyPrices GetLatestPrices(string securityCode, DateTime date);

        IEnumerable<Security> SearchByCode(string searchTerm);
    }
}
