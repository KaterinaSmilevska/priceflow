using DataAccess.Models;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using PriceFlowApp.DTOs;
using System.Linq;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace PriceFlowApp.Services
{
    public class TransactionsService : ITransactionsService
    {
        private readonly ITransactionsRepository _transactionsRepository;
        private readonly ISecuritiesRepository _securitiesRepository;
        private readonly IPortfolioReturnsService _portfolioReturnsService;

        public TransactionsService(ITransactionsRepository transactionsRepository, ISecuritiesRepository securitiesRepository,
            IPortfolioReturnsService portfolioReturnsService)
        {
            _transactionsRepository = transactionsRepository;
            _securitiesRepository = securitiesRepository;
            _portfolioReturnsService = portfolioReturnsService;
        }

        public async Task<Transaction> AddAsync(int portfolioId, Transaction transaction)
        {
            HartiiOdVrednost security = await _securitiesRepository.GetByCodeAsync(transaction.HVCode)
                ?? throw new InvalidOperationException($"Security with code  '{transaction.HVCode}' not found.");

            await ValidateSharesAsync(portfolioId, security, transaction);

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
                ?? throw new InvalidOperationException($"Security with code  '{transaction.HVCode}' not found.");

            await ValidateSharesAsync(foundTransaction.PortfolioId, security, transaction, id);

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

        public async Task<PortfolioAnalytics> GetAnalyticsAsync(int portfolioId, bool isReal)
        {
            List<Transakcii> transactions = await _transactionsRepository.GetByPortfolioIdAsync(portfolioId);

            transactions = transactions
                .Where(t => t.Realna == isReal)
                .ToList();

            List<Transakcii> sellTransactions = transactions.FindAll(t => t.TipTransakcija == "Продавање");

            List<Transakcii> buyTransactions = transactions.FindAll(t => t.TipTransakcija == "Купување");

            PortfolioReturnsSummary summary = await _portfolioReturnsService.CalculateSummaryAsync(portfolioId);

            decimal totalRevenue = sellTransactions.Sum(t => t.Iznos);
            decimal totalExpenses = buyTransactions.Sum(t => t.Iznos + CalculateCommission(t)) + 
                sellTransactions.Sum(t => CalculateCommission(t));

            decimal taxes = 0;
            
            if(isReal)
            {
                totalRevenue += summary.TotalDividends;
                taxes = summary.TotalTaxes;

            }
            return new PortfolioAnalytics
            {
                TotalRevenue = totalRevenue,
                TotalExpenses = totalExpenses,
                Balance = totalRevenue - totalExpenses,
                Taxes = taxes,
                IsReal = isReal
            };
        }

        private decimal CalculateTransactionAmount(Transaction transaction)
        {
            return transaction.SharesQuantity * transaction.SharesUnitPrice;

            //decimal feesPercent = transaction.BrokerageCommission
            //       + transaction.StockExchangeCommission
            //       + transaction.CDHVCommission;
            //decimal fees = amount * feesPercent / 100;

            //return transaction.TypeTransaction == "Купување"
            //    ? amount + fees
            //    : amount - fees;

        }

        private decimal CalculateCommission(Transakcii transaction) {
            decimal amount = transaction.KolicinaAkcii * transaction.EdinecnaCenaAkcija;

                decimal feesPercent = transaction.BrokerskaProvizija
                    + transaction.BerzanskaProvizija
                    + transaction.Cdhvprovizija;
                decimal fees = amount * feesPercent / 100;

            return fees;
        }

        private async Task ValidateSharesAsync(int portfolioId, HartiiOdVrednost security,
            Transaction transaction, int? transactionIdToExclude = null)
        {
            if (transaction.SharesQuantity <= 0)
                throw new InvalidOperationException("Transaction quantity must be greater than 0.");

            List<Transakcii> allTransactions = await _transactionsRepository.GetByPortfolioIdAsync(portfolioId);

            allTransactions = allTransactions
                .Where(t => t.Hvid == security.Id && t.Realna == transaction.IsReal)
                .ToList();

            if (transactionIdToExclude.HasValue)
            {
                allTransactions = allTransactions
                    .Where(t => t.Id != transactionIdToExclude.Value)
                    .ToList();
            }

            int totalBought = allTransactions
                .Where(t => t.TipTransakcija == "Купување")
                .Sum(t => t.KolicinaAkcii);

            int totalSold = allTransactions
                .Where(t => t.TipTransakcija == "Продавање")
                .Sum(t => t.KolicinaAkcii);

            int owned = totalBought - totalSold;

            if(transaction.TypeTransaction == "Продавање" && transaction.SharesQuantity > owned)
            {
                throw new InvalidOperationException($"Cannot sell {transaction.SharesQuantity} shares of '{security.Kod}'." +
                    $"You own only {owned}.");
            }

            if(transaction.TypeTransaction == "Купување" && totalBought + transaction.SharesQuantity > security.VkupenBrojAkcii)
            {
                int available = security.VkupenBrojAkcii - totalBought;
                throw new InvalidOperationException($"Cannot buy {transaction.SharesQuantity} shares of '{security.Kod}'." +
                    $"Only {available} available.");
            }
        }

        public async Task<int> FindOwnedSharesAsync(int portfolioId, string securityCode, bool isReal)
        {
            HartiiOdVrednost security = await _securitiesRepository.GetByCodeAsync(securityCode)
                ?? throw new InvalidOperationException("Security not found.");

            return await _transactionsRepository
                .GetOwnedSharesAsync(portfolioId, security.Id, isReal);
        }
    }
}
