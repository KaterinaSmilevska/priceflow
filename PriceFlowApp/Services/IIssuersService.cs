using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public interface IIssuersService
    {
        Task<IEnumerable<Issuer>> FindAllAsync();
    }
}
