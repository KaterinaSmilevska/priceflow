using DataAccess.Models;
using DataAccess.Repositories;
using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public class TypeSecurityService : ITypeSecurityService
    {
        private readonly ITypeSecurityRepository _typeSecurityRepository;

        public TypeSecurityService(ITypeSecurityRepository typeSecurityRepository)
        {
            _typeSecurityRepository = typeSecurityRepository;
        }

        public async Task<IEnumerable<TypeSecurity>> FindAllAsync()
        {
            IEnumerable<TipHv> types = await _typeSecurityRepository.GetAllAsync();

            return types.Select(i => new TypeSecurity
            {
                Id = i.Id,
                Name = i.Ime
            });
        }
    }
}
