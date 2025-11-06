using DataAccess.Models;
using DataAccess.Repositories;

namespace PriceFlowApp.Services
{
    public class RolesService: IRolesService
    {
        private readonly IRolesRepository _rolesRepository;

        public RolesService(IRolesRepository rolesRepository)
        {
            _rolesRepository = rolesRepository;
        }

        public async Task<Ulogi?> FindByNameAsync(string name)
        {
            var response = await _rolesRepository.GetByNameAsync(name);
            if(response == null)
                throw new Exception("No role found");
            return response;

        }

        public async Task<List<string>> FindNamesAsync()
        {
            return await _rolesRepository.GetNamesAsync();
        }
    }
}
