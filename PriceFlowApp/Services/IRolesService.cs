using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public interface IRolesService
    {
        List<string> FindByUserId(int userId);

        Role FindByName(string name);

        List<string> FindNames();
    }
}
