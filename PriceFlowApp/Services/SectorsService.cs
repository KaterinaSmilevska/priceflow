using DataAccess.Models;
using DataAccess.Repositories;
using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public class SectorsService : ISectorsService
    {
        private readonly ISectorsRepository _sectorsRepository;

        public SectorsService(ISectorsRepository sectorsRepository)
        {
            _sectorsRepository = sectorsRepository;
        }
           
        public IEnumerable<Sector> FindAll()
        {
            IEnumerable<Sektori> issuers = _sectorsRepository.GetAll();

            return issuers.Select(i => new Sector
            {
                SectorId = i.Id,
                SectorName = i.Ime
            });
        }
    }
}
