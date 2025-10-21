using DataAccess.Models;
using DataAccess.Repositories;
using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public class UlogaService
    {
        private readonly PriceFlowDbContext _context;
        private readonly UlogaRepository _ulogaRepository;

        public UlogaService(PriceFlowDbContext context, UlogaRepository ulogaRepository)
        {
            _context = context;
            _ulogaRepository = ulogaRepository;
        }

        public async Task<Uloga> GetUlogaAsync(String name)
        {
            var response = await _ulogaRepository.GetByNameAsync(name);
            if(response == null)
                throw new Exception("No role found");
            return response;

        }
    }
}
