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

            return issuers.Select(MapToIssuer);
        }

        private Issuer MapToIssuer(Izdavachi issuer)
        {
            return new Issuer
            {
                Id = issuer.Id,
                Name = issuer.Ime
            };
        }
    }
}
