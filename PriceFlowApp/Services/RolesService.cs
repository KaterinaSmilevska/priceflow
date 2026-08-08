using DataAccess.Models;
using DataAccess.Repositories;
using PriceFlowApp.DTOs;
using PriceFlowApp.Exceptions;
using PriceFlowApp.Helpers;

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

        public Role FindByName(string name)
        {
            ValidationHelper.ValidateRequiredField(name, "Name", "NAME_VALIDATION_REQUIRED");

            Ulogi role = GetRoleByName(name);

            return MapToRole(role);
        }

        public List<string> FindByUserId(int userId)
        {
            Korisnici user = GetUserById(userId);

            return _rolesRepository.GetByUserId(userId);
        }

        public List<string> FindNames()
        {
            return _rolesRepository.GetNames();
        }

        private Ulogi GetRoleByName(string name)
        {
            Ulogi? role = _rolesRepository.GetByName(name);
            if (role == null)
                throw new NotFoundException("ROLE_NOT_FOUND", "Role not found.");

            return role;
        }

        private Korisnici GetUserById(int userId)
        {
            Korisnici? user = _authRepository.GetById(userId);
            if (user == null)
                throw new NotFoundException("USER_NOT_FOUND", "User not found.");

            return user;
        }

        private Role MapToRole(Ulogi role)
        {
            return new Role
            {
                Id = role.Id,
                Name = role.Ime
            };
        }
    }
}
