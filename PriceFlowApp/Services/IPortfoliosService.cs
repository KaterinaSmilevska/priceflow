using DataAccess.Models;
using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public interface IPortfoliosService
    {
        Task<IEnumerable<PortfolioList>> FindUserPortfoliosAsync(int userId);

        Task<PortfolioDetails?> FindPortfolioAsync(int id, int userId);

        Task<PortfolioList?> CreatePortfolio(int userId, CreatePortfolio portfolio);

        Task<PortfolioList?> UpdatePortfolio(int id, int userId, UpdatePortfolio portfolio);

        Task DeletePortfolio(int id, int userId);
    }
}
