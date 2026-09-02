using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public interface IThresholdService
    {
        IEnumerable<ThresholdResponse> GetUserThresholds(int userId);

        IEnumerable<OwnedSecurity> GetOwnedSecurities(int userId);

        ThresholdResponse Add(int userId, AddThresholdRequest request);

        ThresholdResponse Update(int userId, int id, UpdateThresholdRequest request);

        ThresholdResponse Delete(int userId, int id);
    }
}
