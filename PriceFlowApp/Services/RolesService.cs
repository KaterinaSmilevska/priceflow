using DataAccess.Models;
using DataAccess.Repositories;
using PriceFlowApp.Exceptions;

namespace PriceFlowApp.Services
{
    public class RolesService: IRolesService
    {
        private readonly IRolesRepository _rolesRepository;
        private readonly IAuthRepository _authRepository;

        public RolesService(IRolesRepository rolesRepository, IAuthRepository authRepository)
        {
            _rolesRepository = rolesRepository;
            _authRepository = authRepository;
        }

        public Ulogi? FindByName(string name)
        {
            if(string.IsNullOrWhiteSpace(name))
                throw new ValidationException("ROLE_NAME_REQUIRED", "Name cannot be null or empty.");

            Ulogi? roles = _rolesRepository.GetByName(name);
            if(roles == null)
                throw new NotFoundException("ROLE_NOT_FOUND", "Role not found.");

            return roles;
        }

        public List<string> FindByUserId(int userId)
        {
            Korisnici? user = _authRepository.GetById(userId);
            if (user == null)
                throw new NotFoundException("USER_NOT_FOUND", "User not found.");

            return _rolesRepository.GetByUserId(userId);
        }

        public List<string> FindNames()
        {
            return _rolesRepository.GetNames();
        }
    }
}
