using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public interface IAuthRepository
    {
        public Task<Korisnik?> GetByUsernameAsync(string username);
        Task<Korisnik?> GetKorisnikByUsernameAsync(string username);
        Task<IEnumerable<Korisnik>> getAllKorisnikAsync();
        Task AddKorisnikAsync(Korisnik korisnik);
        Task AddKorisnikUlogaAsync(KorisnikUloga korisnikUloga);
        Task<IEnumerable<Uloga>> GetAllUlogasAsync();
        Task<List<string>> GetUlogaNamesAsync();
        Task<List<int>> GetUlogaIdsByNamesAsync(List<string> ulogaNames);
        Task<bool> UsernameExistsAsync(string username);
        Task<List<Uloga>> GetUlogasForKorisnik(int korisnikId);
        Task<List<string>> GetUlogaNames(List<Uloga> ulogas);
    }
}
