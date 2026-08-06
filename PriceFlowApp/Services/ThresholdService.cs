using DataAccess.Models;
using DataAccess.Repositories;
using DocumentFormat.OpenXml.Office2010.Excel;
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
            DataAccess.Models.Korisnici user = GetUser(userId);

            IEnumerable<HvPromenaCena?> entities = _thresholdRepository.GetByUserId(userId);

            return entities.Select(e => new ThresholdResponse
            {
                Id = e.Id,
                HvId = e.Hvid,
                HvCode = e.Hv.Kod,
                LowerThreshold = e.DolnaGranica,
                UpperThreshold = e.GornaGranica
            }).ToList();
        }

        public IEnumerable<OwnedSecurity> GetOwnedSecurities(int userId)
        {
            DataAccess.Models.Korisnici user = GetUser(userId);

            List<int> ownedIds = _transactionsRepository.GetOwnedSecuritiesIds(userId);

            IEnumerable<HartiiOdVrednost?> securities = _securitiesRepository.GetAllByIds(ownedIds);

            return securities.Select(s => new OwnedSecurity
            {
                Id = s.Id,
                hvCode = s.Kod
            }).ToList();
        }

        public ThresholdResponse Add(int userId, AddThresholdRequest request)
        {
            if (request.LowerThreshold >= request.UpperThreshold)
                throw new ValidationException("INVALID_THRESHOLD_RANGE", "Lower threshold must be less than upper threshold.");

            DataAccess.Models.Korisnici user = GetUser(userId);
            HartiiOdVrednost security = GetSecurity(request.HvId);

            HvPromenaCena? existingThreshold = _thresholdRepository.GetByUserIdAndSecurityCode(userId, request.HvId);

            if (existingThreshold != null)
                throw new AlreadyExistsException("THRESHOLD_ALREADY_EXISTS", "Threshold already exists for this security.");

            HvPromenaCena entity = new HvPromenaCena
            {
                KorisnikId = userId,
                Hvid = request.HvId,
                DolnaGranica = request.LowerThreshold,
                GornaGranica = request.UpperThreshold
            };

             HvPromenaCena createdThreshold = _thresholdRepository.Add(entity);

            return new ThresholdResponse
            {
                Id = createdThreshold.Id,
                HvId = createdThreshold.Hvid,
                HvCode = createdThreshold.Hv.Kod,
                LowerThreshold = createdThreshold.DolnaGranica,
                UpperThreshold = createdThreshold.GornaGranica
            };
        }

        public ThresholdResponse Update(int userId, int id, UpdateThresholdRequest request)
        {
            HvPromenaCena existingThreshold = GetThreshold(id);

            if (existingThreshold.KorisnikId != userId)
                throw new UnauthorizedException("THRESHOLD_ACCESS_DENIED", "You do not have access to this threshold.");

            if (request.LowerThreshold >= request.UpperThreshold)
                throw new ValidationException("INVALID_THRESHOLD_RANGE", "Lower threshold must br less than upper threshodl.");

            existingThreshold.DolnaGranica = request.LowerThreshold;
            existingThreshold.GornaGranica = request.UpperThreshold;
            existingThreshold.DateModified = DateTime.Now;

            HvPromenaCena updatedThreshold = _thresholdRepository.Update(existingThreshold);

            return new ThresholdResponse
            {
                Id = existingThreshold.Id,
                HvId = existingThreshold.Hvid,
                HvCode = existingThreshold.Hv.Kod,
                LowerThreshold = existingThreshold.DolnaGranica,
                UpperThreshold = existingThreshold.GornaGranica
            };
        }

        public ThresholdResponse Delete(int userId, int id)
        {
            HvPromenaCena existingThreshold = GetThreshold(id);

            if (existingThreshold.KorisnikId != userId)
                throw new UnauthorizedException("THRESHOLD_ACCESS_DENIED", "You do not have access to this threshold.");

            _thresholdRepository.Delete(existingThreshold);

            return new ThresholdResponse
            {
                Id = existingThreshold.Id,
                HvId = existingThreshold.Hvid,
                HvCode = existingThreshold.Hv.Kod,
                LowerThreshold = existingThreshold.DolnaGranica,
                UpperThreshold = existingThreshold.GornaGranica
            };
        }

        private DataAccess.Models.Korisnici GetUser(int userId)
        {
            var user = _authRepository.GetById(userId);
            if (user == null)
                throw new NotFoundException("USER_NOT_FOUND", "User not found.");

            return user;
        }

        private HartiiOdVrednost GetSecurity(int securityId)
        {
            var security = _securitiesRepository.GetById(securityId);
            if (security == null)
                throw new NotFoundException("SECURITY_NOT_FOUND", "Security not found.");

            return security;
        }

        private HvPromenaCena GetThreshold(int thresholdId)
        {
            HvPromenaCena? threshold = _thresholdRepository.GetById(thresholdId);

            if (threshold == null)
                throw new NotFoundException("THRESHOLD_NOT_FOUND", "Threshold not found");

            return threshold;
        }
    }
}
