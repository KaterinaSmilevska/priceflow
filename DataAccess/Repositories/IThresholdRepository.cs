using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public interface IThresholdRepository
    {
        Task<IEnumerable<HvPromenaCena>> GetByUserAsync(int userId);

        Task<HvPromenaCena?> GetByIdAsync(int id);

        Task<HvPromenaCena?> GetByUserandSecurityCodeAsync(int userId, int securityId);

        Task<HvPromenaCena> AddAsync(HvPromenaCena entity);

        Task<HvPromenaCena> UpdateAsync(HvPromenaCena entity);

        Task DeleteAsync(HvPromenaCena entity);
    }
}
