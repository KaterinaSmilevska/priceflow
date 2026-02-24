using DataAccess.Models;
using DataAccess.Repositories;
using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public class ThresholdService : IThresholdService
    {
        private readonly IThresholdRepository _thresholdRepository;
        private readonly ITransactionsRepository _transactionsRepository;
        private readonly ISecuritiesRepository _securitiesRepository;

        public ThresholdService(IThresholdRepository thresholdRepository, ITransactionsRepository transactionsRepository,
            ISecuritiesRepository securitiesRepository)
        {
            _thresholdRepository = thresholdRepository;
            _transactionsRepository = transactionsRepository;
            _securitiesRepository = securitiesRepository;
        }

        public async Task<IEnumerable<ThresholdResponse>> GetUserThresholdsAsync(int userId)
        {
            IEnumerable<HvPromenaCena> entities = await _thresholdRepository.GetByUserAsync(userId);

            return entities.Select(e => new ThresholdResponse
            {
                Id = e.Id,
                HvId = e.Hvid,
                HvCode = e.Hv.Kod,
                LowerThreshold = e.DolnaGranica,
                UpperThreshold = e.GornaGranica
            }).ToList();
        }

        public async Task AddAsync(int userId, CreateThresholdRequest request)
        {
            if (request.LowerThreshold >= request.UpperThreshold)
                throw new Exception("Lower threshold must be less than upper threshold.");

            HvPromenaCena? existingThreshold = await _thresholdRepository.GetByUserandSecurityCodeAsync(userId, request.HvId);

            if (existingThreshold != null)
                throw new Exception("Threshold already exists for this security");

            HvPromenaCena entity = new HvPromenaCena
            {
                KorisnikId = userId,
                Hvid = request.HvId,
                DolnaGranica = request.LowerThreshold,
                GornaGranica = request.UpperThreshold
            };
            await _thresholdRepository.AddAsync(entity);
        }

        public async Task UpdateAsync(int userId, int id, UpdateThresholdRequest request)
        {
            HvPromenaCena? entity = await _thresholdRepository.GetByIdAsync(id);

            if (entity == null || entity.KorisnikId != userId)
                throw new Exception("Threshold not found");

            if (request.LowerThreshold >= request.UpperThreshold)
                throw new Exception("Lower threshold must br less than upper threshodl.");

            entity.DolnaGranica = request.LowerThreshold;
            entity.GornaGranica = request.UpperThreshold;
            entity.DateModified = DateTime.Now;

            await _thresholdRepository.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int userId, int id)
        {
            HvPromenaCena? entity = await _thresholdRepository.GetByIdAsync(id);

            if (entity == null || entity.KorisnikId != userId)
                throw new Exception("Threshold not found.");

            await _thresholdRepository.DeleteAsync(entity);
        }

        public async Task<IEnumerable<OwnedSecurity>> GetOwnedSecuritiesAsync(int userId)
        {
            List<int> ownedIds = await _transactionsRepository.GetOwnedSecuritiesIdsAsync(userId);

            IEnumerable<HartiiOdVrednost> securities = await _securitiesRepository.GetAllByIds(ownedIds);
            return securities.Select(s => new OwnedSecurity
            {
                Id = s.Id,
                hvCode = s.Kod
            })
            .ToList();
        }
    }
}
