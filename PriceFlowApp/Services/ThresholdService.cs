using DataAccess.Models;
using DataAccess.Repositories;
using PriceFlowApp.DTOs;
using PriceFlowApp.Exceptions;

namespace PriceFlowApp.Services
{
    public class ThresholdService : IThresholdService
    {
        private readonly IThresholdRepository _thresholdRepository;
        private readonly ITransactionsRepository _transactionsRepository;
        private readonly ISecuritiesRepository _securitiesRepository;
        private readonly IAuthRepository _authRepository;

        public ThresholdService(IThresholdRepository thresholdRepository, ITransactionsRepository transactionsRepository,
            ISecuritiesRepository securitiesRepository, IAuthRepository authRepository)
        {
            _thresholdRepository = thresholdRepository;
            _transactionsRepository = transactionsRepository;
            _securitiesRepository = securitiesRepository;
            _authRepository = authRepository;
        }

        public IEnumerable<ThresholdResponse> GetUserThresholds(int userId)
        {
            Korisnici user = GetUserById(userId);

            IEnumerable<HvPromenaCena> thresholds = _thresholdRepository.GetByUserId(user.Id);

            return thresholds.
                Select(MapToThreshold)
                .ToList();
        }

        public IEnumerable<OwnedSecurity> GetOwnedSecurities(int userId)
        {
            Korisnici user = GetUserById(userId);

            List<int> ownedIds = _transactionsRepository.GetOwnedSecuritiesIds(user.Id);

            IEnumerable<HartiiOdVrednost> securities = _securitiesRepository.GetAllByIds(ownedIds);

            return securities.Select(s => new OwnedSecurity
            {
                Id = s.Id,
                SecurityCode = s.Kod
            }).ToList();
        }

        public ThresholdResponse Add(int userId, AddThresholdRequest request)
        {
            if (request.LowerThreshold >= request.UpperThreshold)
                throw new ValidationException("INVALID_THRESHOLD_RANGE", "Lower threshold must be less than upper threshold.");

            ValidateThresholdAvailability(userId, request.SecurityId);

            Korisnici user = GetUserById(userId);
            HartiiOdVrednost security = GetSecurityById(request.SecurityId);

            HvPromenaCena threshold = new HvPromenaCena
            {
                KorisnikId = user.Id,
                Hvid = security.Id,
                DolnaGranica = request.LowerThreshold,
                GornaGranica = request.UpperThreshold
            };

            HvPromenaCena addedThreshold = _thresholdRepository.Add(threshold);

            return MapToThreshold(addedThreshold);
        }

        public ThresholdResponse Update(int userId, int id, UpdateThresholdRequest request)
        {
            ValidateThresholdRange(request);
            ValidateThresholdAvailability(userId, request.SecurityId, id);

            HvPromenaCena existingThreshold = GetThresholdById(id);

            if (existingThreshold.KorisnikId != userId)
                throw new UnauthorizedException("THRESHOLD_ACCESS_DENIED", "You do not have access to this threshold.");

            existingThreshold.DolnaGranica = request.LowerThreshold;
            existingThreshold.GornaGranica = request.UpperThreshold;
            existingThreshold.DateModified = DateTime.Now;

            HvPromenaCena updatedThreshold = _thresholdRepository.Update(existingThreshold);

            return MapToThreshold(updatedThreshold);
        }

        public ThresholdResponse Delete(int userId, int id)
        {
            HvPromenaCena existingThreshold = GetThresholdById(id);

            if (existingThreshold.KorisnikId != userId)
                throw new UnauthorizedException("THRESHOLD_ACCESS_DENIED", "You do not have access to this threshold.");

            HvPromenaCena deletedThreshold = _thresholdRepository.Delete(existingThreshold);

            return MapToThreshold(deletedThreshold);
        }

        private Korisnici GetUserById(int userId)
        {
            Korisnici? user = _authRepository.GetById(userId);
            if (user == null)
                throw new NotFoundException("USER_NOT_FOUND", "User not found.");

            return user;
        }

        private HartiiOdVrednost GetSecurityById(int securityId)
        {
            HartiiOdVrednost? security = _securitiesRepository.GetById(securityId);
            if (security == null)
                throw new NotFoundException("SECURITY_NOT_FOUND", "Security not found.");

            return security;
        }

        private HvPromenaCena GetThresholdById(int thresholdId)
        {
            HvPromenaCena? threshold = _thresholdRepository.GetById(thresholdId);
            if (threshold == null)
                throw new NotFoundException("THRESHOLD_NOT_FOUND", "Threshold not found");

            return threshold;
        }

        private void ValidateThresholdRange(UpdateThresholdRequest request)
        {
            if (request.LowerThreshold >= request.UpperThreshold)
                throw new ValidationException("INVALID_THRESHOLD_RANGE", "Lower threshold must bе less than upper threshold.");
        }

        private void ValidateThresholdAvailability(int userId, int securityId, int? thresholdId = null)
        {
            HvPromenaCena? existingThreshold = _thresholdRepository.GetByUserIdAndSecurityId(userId, securityId);
            if (existingThreshold != null && existingThreshold.Id != thresholdId)
                throw new AlreadyExistsException("THRESHOLD_ALREADY_EXISTS", "Threshold already exists for this security.");
        }

        private ThresholdResponse MapToThreshold(HvPromenaCena threshold)
        {
            return new ThresholdResponse
            {
                Id = threshold.Id,
                SecurityId = threshold.Hvid,
                SecurityCode = threshold.Hv.Kod,
                LowerThreshold = threshold.DolnaGranica,
                UpperThreshold = threshold.GornaGranica
            };
        }
    }
}
