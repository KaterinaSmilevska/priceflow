using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public interface ITypeSecurityService
    {
        IEnumerable<TypeSecurity> FindAll();
    }
}
