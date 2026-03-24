using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public interface ISectorsService
    {
        Task<IEnumerable<Sector>> FindAllAsync();
    }
}
