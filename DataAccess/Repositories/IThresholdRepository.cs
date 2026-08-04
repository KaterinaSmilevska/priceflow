using DataAccess.Models;

namespace DataAccess.Repositories
{
    public interface IThresholdRepository
    {
        HvPromenaCena? GetById(int id);

        IEnumerable<HvPromenaCena?> GetByUserId(int userId);

        HvPromenaCena? GetByUserIdAndSecurityCode(int userId, int securityId);

        HvPromenaCena Add(HvPromenaCena entity);

        HvPromenaCena Update(HvPromenaCena entity);

        HvPromenaCena Delete(HvPromenaCena entity);
    }
}
