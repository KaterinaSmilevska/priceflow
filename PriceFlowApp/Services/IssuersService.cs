using DataAccess.Models;
using DataAccess.Repositories;
using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public class IssuersService : IIssuersService
    {
        private readonly IIssuersRepository _issuersRepository;

        public IssuersService(IIssuersRepository issuersRepository) => _issuersRepository = issuersRepository;

        public async Task<IEnumerable<Issuer>> FindAllAsync()
        {
            IEnumerable<Izdavachi> issuers = await _issuersRepository.GetAllAsync();

            return issuers.Select(i => new Issuer
            {
                Id = i.Id,
                Name = i.Ime
            });
        }
    }
}
