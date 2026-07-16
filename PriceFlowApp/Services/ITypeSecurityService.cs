using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public interface ITypeSecurityService
    {
        Task<IEnumerable<TypeSecurity>> FindAllAsync();
    }
}
