using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public interface ISectorsService
    {
        IEnumerable<Sector> FindAll();
    }
}
