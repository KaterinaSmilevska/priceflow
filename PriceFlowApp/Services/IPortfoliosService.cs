using DataAccess.Models;
using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public interface IPortfoliosService
    {
        Task<IEnumerable<Portfolio>> FindUserPortfoliosAsync(int userId);

        Task<Portfolio> FindById(int id);

        Task<Portfolio> CreatePortfolio(int userId, CreatePortfolio portfolio);

        Task<Portfolio> UpdatePortfolio(int id, int userId, UpdatePortfolio portfolio);

        Task DeletePortfolio(int id, int userId);
    }
}
