using DataAccess.Models;

namespace DataAccess.Repositories
{
    public interface IAuthRepository
    {
        Task<Korisnici?> GetByIdAsync(int id);

        Task<Korisnici?> GetByUsernameAsync(string username);

        Task<Korisnici?> GetByVerificationTokenAsync(Guid token);

        Task<Korisnici?> GetByEmailAsync(string email);

        Task<Korisnici?> GetByResetPasswordTokenAsync(Guid token);

        Task<IEnumerable<Korisnici>> GetAllAsync();

        Task AddAsync(Korisnici user);

        Task UpdateAsync(Korisnici user);

        Task DeleteAsync(Korisnici user);

        Task<bool> UsernameExistsAsync(string username);        
    }
}
