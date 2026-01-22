using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public interface ISecuritiesRepository
    {
        Task<IEnumerable<HartiiOdVrednost>> GetAllAsync();

        Task<HartiiOdVrednost?> GetByIdAsync(int id);

        Task<HartiiOdVrednost?> GetByCodeAsync(string code);

        Task<HartiiOdVrednost> AddAsync(HartiiOdVrednost security);

        Task DeleteAsync(int id);

        Task UpdateAsync(HartiiOdVrednost security);
    }
}
