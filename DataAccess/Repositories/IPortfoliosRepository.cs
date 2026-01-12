using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public interface IPortfoliosRepository
    {
        Task<Portfolija?> GetByIdAsync(int id, int userId);

        Task<IEnumerable<Portfolija>> GetByUserAsync(int userId);

        Task<Portfolija> CreateAsync(Portfolija portfolio);

        Task<Portfolija?> UpdateAsync(Portfolija portfolio);

        Task DeleteAsync(int id, int userId);
    }
}
