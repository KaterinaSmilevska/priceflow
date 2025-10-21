using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class UlogaRepository
    {
        private readonly PriceFlowDbContext _context;
        public UlogaRepository(PriceFlowDbContext context) => _context = context;

        public async Task<Uloga?> GetByNameAsync(string name) =>
            await _context.Ulogas.FirstOrDefaultAsync(u => u.Ime == name);
    }
}
