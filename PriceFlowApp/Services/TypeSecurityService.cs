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

        public IEnumerable<TypeSecurity> FindAll()
        {
            IEnumerable<TipHv> types = _typeSecurityRepository.GetAll();

            return types
                .Select(MapToTypeSecurity)
                .ToList();
        }

        private TypeSecurity MapToTypeSecurity(TipHv typeSecurity)
        {
            return new TypeSecurity
            {
                Id = typeSecurity.Id,
                Name = typeSecurity.Ime
            };
        }
    }
}
