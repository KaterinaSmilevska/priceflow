using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class AuthRepository: IAuthRepository
    {
        private readonly PriceFlowDbContext _dbContext;
        public AuthRepository(PriceFlowDbContext context) => _dbContext = context;

        public async Task<Korisnik?> GetByUsernameAsync(string username)
        {
            return await _dbContext.Korisniks.FirstOrDefaultAsync(k => k.Username == username);
        }

        public async Task<Korisnik?> GetKorisnikByUsernameAsync(string username)
        {
            return await _dbContext.Korisniks
                .FirstOrDefaultAsync(k => k.Username == username)
                ?? throw new ArgumentException($"Korisnik with username '{username}' not found.");
        }

        public async Task<IEnumerable<Korisnik>> getAllKorisnikAsync()
        {
            return await _dbContext.Korisniks.ToListAsync();
        }

        public async Task AddKorisnikAsync(Korisnik korisnik)
        {
            _dbContext.Korisniks.Add(korisnik);
            await _dbContext.SaveChangesAsync();
        }

        public async Task AddKorisnikUlogaAsync(KorisnikUloga korisnikUloga)
        {
            _dbContext.KorisnikUlogas.Add(korisnikUloga);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<string>> GetUlogaNamesAsync()
        {
            return await _dbContext.Ulogas.Select(u => u.Ime).ToListAsync();
        }

        public async Task<List<int>> GetUlogaIdsByNamesAsync(List<string> ulogaNames)
        {
            return await _dbContext.Ulogas
               .Where(u => ulogaNames.Contains(u.Ime))
               .Select(u => u.Id)
               .ToListAsync();
        }

        public async Task<IEnumerable<Uloga>> GetAllUlogasAsync()
        {
            return await _dbContext.Ulogas.ToListAsync();
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _dbContext.Korisniks.AnyAsync(k => k.Username == username);

        }

        public async Task<List<Uloga>> GetUlogasForKorisnik(int korisnikId)
        {
            return await _dbContext.KorisnikUlogas
                .Where(ku => ku.KorisnikId == korisnikId)
                .Select(ku => ku.Uloga)
                .ToListAsync();
        }

        public async Task<List<string>> GetUlogaNames(List<Uloga> ulogas)
        {
            return await _dbContext.Ulogas
               .Select(u => u.Ime)
               .ToListAsync();
        }
    }
}
