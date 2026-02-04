using DataAccess.Models;
using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public interface ISecuritiesService
    {
        Task<IEnumerable<Security>> FindAllAsync();

        Task<Security?> FindByIdAsync(int id);

        Task<Security?> FindByCodeAsync(string code);

        Task<string?> FindSecurityCode(int id);

        Task<int?> FindTotalNumShares(int id);

        Task<int?> FindTotalNumSharesAsync(string securityCode);

        Task<Security> AddAsync(CreateSecurity security);

        Task DeleteAsync(int id);

        Task<Security> UpdateAsync(int id, CreateSecurity security);
    }
}
