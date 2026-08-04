using DataAccess.Models;
using DataAccess.Repositories;
using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public class IssuersService : IIssuersService
    {
        private readonly IIssuersRepository _issuersRepository;

        public IssuersService(IIssuersRepository issuersRepository)
        {
            _issuersRepository = issuersRepository;
        }

        public IEnumerable<Issuer> FindAll()
        {
            IEnumerable<Izdavachi> issuers = _issuersRepository.GetAll();

            return issuers.Select(i => new Issuer
            {
                Id = i.Id,
                Name = i.Ime
            });
        }
    }
}
