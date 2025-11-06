using DataAccess.Models;
using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public interface ISecuritiesService
    {
        Task<IEnumerable<Security>> FindAllAsync();

        Task<Security?> FindByIdAsync(int id);
    }
}
