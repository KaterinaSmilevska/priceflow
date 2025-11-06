using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public interface IIssuersRepository
    {
        Task<Izdavachi?> GetByIdAsync(int id);

        Task<IEnumerable<Izdavachi>> GetAllAsync();
    }
}
