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

            return issuers
                .Select(MapToSector)
                .ToList();
        }

        private Sector MapToSector(Sektori sector)
        {
            return new Sector
            {
                SectorId = sector.Id,
                SectorName = sector.Ime
            };
        }
    }
}
