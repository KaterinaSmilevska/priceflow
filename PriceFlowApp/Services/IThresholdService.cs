using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public interface IThresholdService
    {
        Task<IEnumerable<ThresholdResponse>> GetUserThresholdsAsync(int userId);

        Task AddAsync(int userId, CreateThresholdRequest request);

        Task UpdateAsync(int userId, int id, UpdateThresholdRequest request);

        Task DeleteAsync(int userId, int id);

        Task<IEnumerable<OwnedSecurity>> GetOwnedSecuritiesAsync(int userId);
    }
}
