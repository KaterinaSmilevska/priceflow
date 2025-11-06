using DataAccess.Models;

namespace PriceFlowApp.Services
{
    public interface IRolesService
    {
        Task<Ulogi?> FindByNameAsync(string name);

        Task<List<string>> FindNamesAsync();
    }
}
