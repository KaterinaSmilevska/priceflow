using DataAccess.Models;
using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public interface ISecuritiesService
    {
        Task<IEnumerable<Security>> FindAllAsync();

        Task<Security?> FindByIdAsync(int id);

        Task<Security> AddAsync(CreateSecurity security);

        Task DeleteAsync(int id);

        Task<Security> UpdateAsync(int id, CreateSecurity security);
    }
}
