using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public interface IIssuersService
    {
        IEnumerable<Issuer> FindAll();
    }
}
