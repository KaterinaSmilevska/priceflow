using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PriceFlowApp.DTOs;
using PriceFlowApp.Models;
using PriceFlowSecurity;

namespace PriceFlowApp.Controllers
{
    [ApiController]
    [Route("/api[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            if (await _context.Korisniks.AnyAsync(u => u.Username == registerDTO.Username))
                return BadRequest("Username already exists");

            var salt = PasswordHasher.GenerateSalt();
            string hash = PasswordHasher.HashPassword(registerDTO.Password, salt);

            byte[] hashBytes = Convert.FromBase64String(hash);
            byte[] passwordHash = new byte[48];
            Buffer.BlockCopy(hashBytes, 0, passwordHash, 0, 32);
            Buffer.BlockCopy(salt, 0, passwordHash, 32, 16);

            var user = new Korisnik
            {
                Ime = registerDTO.Ime,
                Username = registerDTO.Username,
                PasswordHash = passwordHash,
                Email = registerDTO.Email,
                KorisnikUlogas = new List<KorisnikUloga>()
            };

            foreach(var ulogaId in registerDTO.UlogaIds)
            {
                user.KorisnikUlogas.Add(new KorisnikUloga { UlogaId = ulogaId });
            }

            _context.Korisniks.Add(user);
            await _context.SaveChangesAsync();

            return Ok("User registered");

        }
        
    }
}
