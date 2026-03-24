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
           
        public async Task<IEnumerable<Sector>> FindAllAsync()
        {
            IEnumerable<Sektori> issuers = await _sectorsRepository.GetAllAsync();

            return issuers.Select(i => new Sector
            {
                SectorId = i.Id,
                SectorName = i.Ime
            });
        }
    }
}
