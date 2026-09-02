using DataAccess.Models;

namespace DataAccess.Repositories
{
    public interface IThresholdRepository
    {
        HvPromenaCena? GetById(int id);

        IEnumerable<HvPromenaCena> GetByUserId(int userId);

        IEnumerable<HvPromenaCena> GetBySecurityId(int securityId);

        HvPromenaCena? GetByUserIdAndSecurityId(int userId, int securityId);

        HvPromenaCena Add(HvPromenaCena entity);

        HvPromenaCena Update(HvPromenaCena entity);

        HvPromenaCena Delete(HvPromenaCena entity);
    }
}
