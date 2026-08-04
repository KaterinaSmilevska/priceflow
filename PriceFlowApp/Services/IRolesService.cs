using DataAccess.Models;

namespace PriceFlowApp.Services
{
    public interface IRolesService
    {
        List<string> FindByUserId(int userId);

        Ulogi? FindByName(string name);

        List<string> FindNames();
    }
}
