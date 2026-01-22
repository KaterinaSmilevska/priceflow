using DataAccess.Models;
using DataAccess.Repositories;
using PriceFlowApp.DTOs;
using System.Reflection.Emit;

namespace PriceFlowApp.Services
{
    public class TransactionsService : ITransactionsService
    {
        private readonly ITransactionsRepository _transactionsRepository;
        private readonly ISecuritiesRepository _securitiesRepository;

        public TransactionsService(ITransactionsRepository transactionsRepository, ISecuritiesRepository securitiesRepository)
        {
            _transactionsRepository = transactionsRepository;
            _securitiesRepository = securitiesRepository;
        }

        public async Task<Transaction> AddAsync(int portfolioId, Transaction transaction)
        {
            HartiiOdVrednost security = await _securitiesRepository.GetByCodeAsync(transaction.HVCode)
                ?? throw new ArgumentException($"Security with code  '{transaction.HVCode}' not found.");
            
            Transakcii entity = new Transakcii
            {
                PortfolioId = portfolioId,
                Hvid = security.Id,
                TipTransakcija = transaction.TypeTransaction,
                KolicinaAkcii = transaction.SharesQuantity,
                Iznos = CalculateTransactionAmount(transaction),
                EdinecnaCenaAkcija = transaction.SharesUnitPrice,
                BerzanskaProvizija = transaction.StockExchangeCommission,
                BrokerskaProvizija = transaction.BrokerageCommission,
                Cdhvprovizija = transaction.CDHVCommission,
                Realna = transaction.IsReal,
                Datum = transaction.Date
            };
             entity = await _transactionsRepository.AddAsync(entity);
            return new Transaction
            {
                Id = entity.Id,
                HVId = entity.Hvid,
                HVCode = entity.Hv.Kod,
                SharesQuantity = entity.KolicinaAkcii,
                SharesUnitPrice = entity.EdinecnaCenaAkcija,
                Amount = entity.Iznos,
                TypeTransaction = entity.TipTransakcija,
                IsReal = entity.Realna,
                StockExchangeCommission = entity.BerzanskaProvizija,
                BrokerageCommission = entity.BrokerskaProvizija,
                CDHVCommission = entity.Cdhvprovizija,
                Date = entity.Datum
            };
        }

        public async Task DeleteAsync(int id)
        {
            Transakcii? transaction = await _transactionsRepository.GetByIdAsync(id);
            if (transaction == null) return;

            await _transactionsRepository.DeleteAsync(transaction);
        }

        public async Task<List<Transaction>> FindByPortfolioIdAsync(int portfolioid)
        {
            List<Transakcii> transactions = await _transactionsRepository.GetByPortfolioIdAsync(portfolioid);

            return transactions.Select(t => new Transaction
            {
                Id = t.Id,
                HVId = t.Hvid,
                HVCode = t.Hv.Kod,
                SharesQuantity = t.KolicinaAkcii,
                SharesUnitPrice = t.EdinecnaCenaAkcija,
                Amount = t.Iznos,
                TypeTransaction = t.TipTransakcija,
                IsReal = t.Realna,
                StockExchangeCommission = t.BerzanskaProvizija,
                BrokerageCommission = t.BrokerskaProvizija,
                CDHVCommission = t.Cdhvprovizija,
                Date = t.Datum
            }).ToList();
        }

        public async Task<Transaction> UpdateAsync(int id, Transaction transaction)
        {
            Transakcii foundTransaction = await _transactionsRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException();

            HartiiOdVrednost security = await _securitiesRepository.GetByCodeAsync(transaction.HVCode)
                ?? throw new ArgumentException($"Security with code  '{transaction.HVCode}' not found.");

            foundTransaction.Hvid = security.Id;
            foundTransaction.KolicinaAkcii = transaction.SharesQuantity;
            foundTransaction.EdinecnaCenaAkcija = transaction.SharesUnitPrice;
            foundTransaction.Iznos = transaction.Amount;
            foundTransaction.TipTransakcija = transaction.TypeTransaction;
            foundTransaction.Realna = transaction.IsReal;
            foundTransaction.BerzanskaProvizija = transaction.StockExchangeCommission;
            foundTransaction.BrokerskaProvizija = transaction.BrokerageCommission;
            foundTransaction.Cdhvprovizija = transaction.CDHVCommission;
            foundTransaction.Datum = transaction.Date;
            foundTransaction.Iznos = CalculateTransactionAmount(transaction);

            var updated = await _transactionsRepository.UpdateAsync(foundTransaction);

            return new Transaction
            {
                HVId = foundTransaction.Hvid,
                HVCode = foundTransaction.Hv.Kod,
                SharesQuantity = foundTransaction.KolicinaAkcii,
                SharesUnitPrice = foundTransaction.EdinecnaCenaAkcija,
                Amount = foundTransaction.Iznos,
                TypeTransaction = foundTransaction.TipTransakcija,
                IsReal = foundTransaction.Realna,
                StockExchangeCommission = foundTransaction.BerzanskaProvizija,
                BrokerageCommission = foundTransaction.BrokerskaProvizija,
                CDHVCommission = foundTransaction.Cdhvprovizija,
                Date = foundTransaction.Datum
            };
        }

        public async Task<PortfolioAnalytics> GetTotalIncomeAsync(int portfolioId)
        {
            List<Transakcii> transactions = await _transactionsRepository.GetByPortfolioIdAsync(portfolioId);

            transactions = transactions.FindAll(t => t.TipTransakcija == "Продавање");

            return new PortfolioAnalytics
            {
                TotalIncome = transactions.Sum(t => t.Iznos)
            };
        }

        private decimal CalculateTransactionAmount(Transaction transaction)
        {
            decimal amount = transaction.SharesQuantity * transaction.SharesUnitPrice;

            decimal feesPercent = transaction.BrokerageCommission
                + transaction.StockExchangeCommission
                + transaction.CDHVCommission;
            decimal fees = amount * feesPercent / 100;

            return transaction.TypeTransaction == "Купување"
                ? amount + fees
                : amount - fees;

        }
    }
}
